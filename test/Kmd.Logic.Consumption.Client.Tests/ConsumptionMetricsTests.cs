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
        public static ConsumptionMetricsDestinationRecord CaptureDestinationRecord(
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

            var capturedSubOwnerContext = new Dictionary<string, string>();
            mockedDestination
                .Setup(d => d.ForSubscriptionOwnerContext(It.IsAny<string>(), It.IsAny<string>()))
                .Callback((string propertyName, string value) => capturedSubOwnerContext.Add(propertyName, value))
                .Returns(mockedDestination.Object);

            var capturedSubscriptionId = default(Guid);
            var capturedResourceId = default(Guid);
            var capturedMeter = default(string);
            var capturedAmount = default(int);
            var capturedReason = default(string);

            mockedDestination
                .Setup(d => d.Write(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Callback((Guid callbackSubscriptionId, Guid callbackResourceId, string callbackMeter, int callbackAmount, string callbackReason) =>
                    (capturedSubscriptionId, capturedResourceId, capturedMeter, capturedAmount, capturedReason) =
                        (callbackSubscriptionId, callbackResourceId, callbackMeter, callbackAmount, callbackReason));

            IConsumptionMetrics consumption = new ConsumptionMetrics(mockedDestination.Object);
            consumption = internalContext?.Aggregate(consumption, (client, kvp) => client.ForInternalContext(kvp.Key, kvp.Value)) ?? consumption;
            consumption = subOwnerContext?.Aggregate(consumption, (client, kvp) => client.ForSubscriptionOwnerContext(kvp.Key, kvp.Value)) ?? consumption;

            consumption.Record(subscriptionId, resourceId, meter, amount, reason);

            return new ConsumptionMetricsDestinationRecord(
                subscriptionId: capturedSubscriptionId,
                resourceId: capturedResourceId,
                meter: capturedMeter,
                amount: capturedAmount,
                reason: capturedReason,
                internalContext: capturedInternalContext,
                subscriptionOwnerContext: capturedSubOwnerContext);
        }

        [Fact]
        public void ConsumptionMetricsRecordsAllValues()
        {
            // Arrange
            var subscriptionId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var meter = "SMS/BYO/Send SMS";
            var amount = 1234;
            var reason = "Any old reason will do";

            var internalContext = new Dictionary<string, string> { { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };
            var subOwnerContext = new Dictionary<string, string> { { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };

            // Act
            var result = CaptureDestinationRecord(
                    subscriptionId: subscriptionId,
                    resourceId: resourceId,
                    meter: meter,
                    amount: amount,
                    reason: reason,
                    internalContext: internalContext,
                    subOwnerContext: subOwnerContext);

            // Assert
            result.Should().BeEquivalentTo(
                new ConsumptionMetricsDestinationRecord(
                    subscriptionId,
                    resourceId,
                    meter,
                    amount,
                    reason,
                    internalContext,
                    subOwnerContext));
        }
    }
}
