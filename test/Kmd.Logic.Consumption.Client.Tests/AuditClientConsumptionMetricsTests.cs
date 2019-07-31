namespace Kmd.Logic.Consumption.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Kmd.Logic.Audit.Client;
    using Kmd.Logic.Consumption.Client.AuditClient;
    using Moq;
    using Xunit;

    public class AuditClientConsumptionMetricsTests
    {
        public static IEnumerable<object[]> GetConsumptionValuesTestCases()
        {

            IReadOnlyList<(string, string)> emptyContext = Array.Empty<(string, string)>();
            return new[]
                {
                    //(subscriptionId: new Guid("00000000-0000-0000-0000-000000000000"),
                    //    resourceId: new Guid("00000000-0000-0000-0000-000000000000"), meter: default(string), amount: 0,
                    //    reason: default(string),
                    //    internalContext: emptyContext,
                    //    subOwnerContext: emptyContext
                    //    ),

                    //(subscriptionId: new Guid("7DB79D7D-18B4-45BA-A5F6-323990D82278"),
                    //    resourceId: new Guid("E7A6693C-4C74-4408-AF09-ECA0C51E4F9C"),
                    //    meter: "SMS/BYO/Sent",
                    //    amount: 1,
                    //    reason: default(string),
                    //    internalContext: emptyContext,
                    //    subOwnerContext: emptyContext),

                    //(subscriptionId: new Guid("06D859D7-0BD1-444D-89D0-5274477F8C75"),
                    //    resourceId: new Guid("FFE473E4-100F-40C6-B31E-FAF996EB2722"), meter: "SMS/Logic/Sent",
                    //    amount: 2, reason: "large, sent as two",
                    //    internalContext: emptyContext,
                    //    subOwnerContext: emptyContext),

                    (subscriptionId: new Guid("06D859D7-0BD1-444D-89D0-5274477F8C75"),
                        resourceId: new Guid("FFE473E4-100F-40C6-B31E-FAF996EB2722"), meter: "SMS/Logic/Sent",
                        amount: 2, reason: "large, sent as two",
                        internalContext: new[]
                        {
                            ("i1", "v1"),
                            ("i2", "v2"),
                        },
                        subOwnerContext: emptyContext),

                    //(subscriptionId: new Guid("06D859D7-0BD1-444D-89D0-5274477F8C75"),
                    //    resourceId: new Guid("FFE473E4-100F-40C6-B31E-FAF996EB2722"), meter: "SMS/Logic/Sent",
                    //    amount: 2, reason: "large, sent as two",
                    //    internalContext: emptyContext,
                    //    subOwnerContext: new[]
                    //    {
                    //        ("s1", "v1"),
                    //        ("s2", "v2"),
                    //    }),

                    //(subscriptionId: new Guid("06D859D7-0BD1-444D-89D0-5274477F8C75"),
                    //    resourceId: new Guid("FFE473E4-100F-40C6-B31E-FAF996EB2722"), meter: "SMS/Logic/Sent",
                    //    amount: 2,
                    //    reason: "both internal and sub owner context",
                    //    internalContext: new[]
                    //    {
                    //        ("i1", "v1"),
                    //        ("i2", "v2"),
                    //    },
                    //    subOwnerContext: new[]
                    //    {
                    //        ("s1", "v1"),
                    //        ("s2", "v2"),
                    //    }),
                }
                .Select(x => new object[] { x.subscriptionId, x.resourceId, x.meter, x.amount, x.reason, x.internalContext, x.subOwnerContext })
                .ToArray();
        }

        [Theory]
        [MemberData(nameof(GetConsumptionValuesTestCases))]
        public void ValuesReachTheAuditDestinationUsingTheTemplate(
            Guid subscriptionId,
            Guid resourceId,
            string meter,
            int amount,
            string reason,
            IReadOnlyList<(string name, string value)> internalContext,
            IReadOnlyList<(string name, string value)> subOwnerContext)
        {
            var auditMock = new Mock<IAudit>();

            auditMock
                .Setup(a => a.Write(It.IsAny<string>(), It.IsAny<object[]>()));

            auditMock
                .Setup(a => a
                    .ForContext(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                .Returns(() => auditMock.Object);

            var subOwnerPropertyGuidPrefix = Guid.NewGuid();
            string SubOwnerPropertyGuidPrefixed(string propertyName) => $"{subOwnerPropertyGuidPrefix}_{propertyName}";
            var consumedDateTime = DateTimeOffset.Now;
            DateTimeOffset GetNow() => consumedDateTime;
            var sut = new AuditClientConsumptionMetricsDestination(auditMock.Object, SubOwnerPropertyGuidPrefixed, GetNow);
            IConsumptionMetrics consumption = new ConsumptionMetrics(sut);

            // act
            foreach (var (name, value) in internalContext)
            {
                consumption = consumption.ForInternalContext(name, value);
            }

            foreach (var (name, value) in subOwnerContext)
            {
                consumption = consumption.ForSubscriptionOwnerContext(name, value);
            }

            consumption.Record(
                subscriptionId: subscriptionId,
                resourceId: resourceId,
                meter: meter,
                amount: amount,
                reason: reason);

            // assert
            var expectedPropertyValues = new object[] { amount, meter, resourceId, subscriptionId, GetNow() };
            var expectedTemplate = AuditClientConsumptionMetricsDestination.Template;
            auditMock.Verify(a => a.Write(expectedTemplate, expectedPropertyValues), times: Times.Once);
            if (reason != null)
            {
                auditMock.Verify(a => a.ForContext("Reason", reason, false), Times.Once);
            }

            foreach (var (name, value) in internalContext)
            {
                auditMock.Verify(a => a.ForContext(name, value, false), Times.Once);
            }

            foreach (var (name, value) in subOwnerContext)
            {
                var subOwnerContextPropertyName = SubOwnerPropertyGuidPrefixed(name);
                auditMock.Verify(a => a.ForContext(subOwnerContextPropertyName, value, false), Times.Once);
            }

            auditMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void TheDefaultTemplateIsCorrect()
        {
            AuditClientConsumptionMetricsDestination.Template.Should()
                .Be("Consumed {Amount} for {Meter} on resource {ResourceId} at {ConsumedDatetime} in subscription {SubscriptionId}");
        }

        [Theory]
        [InlineData("x", "__Sub_x")]
        [InlineData("t", "__Sub_t")]
        [InlineData(" Hello There ", "__Sub_ Hello There ")]
        public void TheDefaultEncodingOfSubscriptionOwnerPropertiesAddsUnderscoreSubPrefix(string name, string expected)
        {
            AuditClientConsumptionMetricsDestination.GetDefaultSubOwnerContextName(name).Should().Be(expected);
        }
    }
}