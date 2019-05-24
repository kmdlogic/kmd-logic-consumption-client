using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ConsumptionMetricsTests
    {
        private readonly ITestOutputHelper output;

        public ConsumptionMetricsTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static ConsumptionMetricsDestinationRecord TestRecord(
               Guid subscriptionId,
               Guid resourceId,
               string meter,
               int amount,
               string reason,
               IDictionary<string, string> internalContext,
               IDictionary<string, string> subOwnerContext)
        {
            var mockedDestination = new Mock<IConsumptionMetricsDestination>();
            var capturedInternalContext = new Dictionary<string, string>();
            mockedDestination
             .Setup(d => d.ForInternalContext(It.IsAny<string>(), It.IsAny<string>()))
             .Callback((string propertyName, string value) => capturedInternalContext.Add(propertyName, value))
             .Returns(mockedDestination.Object);

            var capturedSubContext = new Dictionary<string, string>();

            mockedDestination
             .Setup(d => d.ForSubscriptionOwnerContext(It.IsAny<string>(), It.IsAny<string>()))
             .Callback((string propertyName, string value) => capturedSubContext.Add(propertyName, value))
             .Returns(mockedDestination.Object);

            var capturedMessageTemplate = default(string);

            var capturedArgs = default(object[]);
            mockedDestination
             .Setup(d => d.Write(It.IsAny<string>(), It.IsAny<object[]>()))
             .Callback((string template, object[] args) =>
             {
                 capturedMessageTemplate = template;
                 capturedArgs = args;
             });

            IConsumptionMetrics consumption = new ConsumptionClient(mockedDestination.Object);
            if (internalContext != null)
            {
                consumption = internalContext.Aggregate(consumption, (clientWithContext, kvp) => clientWithContext.ForInternalContext(kvp.Key, kvp.Value));
            }

            if (subOwnerContext != null)
            {
                consumption = subOwnerContext.Aggregate(consumption, (clientWithContext, kvp) => clientWithContext.ForSubscriptionOwnerContext(kvp.Key, kvp.Value));
            }

            consumption.Record(subscriptionId, resourceId, meter, amount, reason);

            return new ConsumptionMetricsDestinationRecord(
                messageTemplate: capturedMessageTemplate,
                args: capturedArgs,
                internalContext: capturedInternalContext,
                subscriptionOwnerContext: capturedSubContext);
        }

        [Fact]
        public void ConsumptionMetricsRecordedInteranlContext()
        {
            // Arrange
            var groupId = Guid.NewGuid().ToString();
            var subscriptionId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var meter = "SMS/BYO/Send SMS";

            Dictionary<string, string> forInternalContext = new Dictionary<string, string>();
            forInternalContext.Add("groupId", groupId);

            // Act
            var result = TestRecord(
                    subscriptionId: subscriptionId,
                    resourceId: resourceId,
                    meter: meter,
                    amount: 1,
                    reason: "Test Consumption",
                    forInternalContext,
                    null);

            // Assert
            result.InternalContext.Should().NotBeEmpty();
            result.InternalContext.Should().ContainKey("groupId");
            result.InternalContext.Should().ContainValue(groupId);
            result.InternalContext.Should().ContainKey("Reason");
            result.InternalContext.Should().ContainValue("Test Consumption");
        }

        [Fact]
        public void ConsumptionMetricsRecordedSubOwnerContextSuccess()
        {
            // Arrange
            var subscriptionId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var meter = "SMS/BYO/Send SMS";
            var resourceType = "SMS Provider";
            var resourceName = "FRIE PROD";

            Dictionary<string, string> subOwnerContext = new Dictionary<string, string>();
            subOwnerContext.Add("Resource Type", resourceType);
            subOwnerContext.Add("Resource Name", resourceName);

            // Act
            var result = TestRecord(
                    subscriptionId: subscriptionId,
                    resourceId: resourceId,
                    meter: meter,
                    amount: 1,
                    reason: "Test Consumption",
                    null,
                    subOwnerContext);

            // Assert
            result.SubscriptionOwnerContext.Should().NotBeEmpty();
            result.SubscriptionOwnerContext.Should().ContainKey("Resource Type");
            result.SubscriptionOwnerContext.Should().ContainValue(resourceType);
            result.SubscriptionOwnerContext.Should().ContainKey("Resource Name");
            result.SubscriptionOwnerContext.Should().ContainValue(resourceName);
        }

        [Fact]
        public void ConsumptionMetricsRecordedMessageTemplateArgsSuccess()
        {
            // Arrange
            var subscriptionId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var meter = "SMS/BYO/Send SMS";
            var messageTemplateArgsCount = 4;

            // Act
            var result = TestRecord(
                    subscriptionId: subscriptionId,
                    resourceId: resourceId,
                    meter: meter,
                    amount: 1,
                    reason: "Test Consumption",
                    null,
                    null);

            // Assert
            result.GetMessageTemplateArgs().Should().NotBeEmpty();
            result.GetMessageTemplateArgs().Length.Should().Be(messageTemplateArgsCount);
            result.GetMessageTemplateArgs().Contains(subscriptionId);
            result.GetMessageTemplateArgs().Contains(resourceId);
            result.GetMessageTemplateArgs().Contains(meter);
        }
    }
}
