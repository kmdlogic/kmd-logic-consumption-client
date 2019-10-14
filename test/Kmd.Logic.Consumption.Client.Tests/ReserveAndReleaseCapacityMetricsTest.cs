using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ReserveAndReleaseCapacityMetricsTest
    {
        public static ReserveOrReleaseCapacityRecord CaptureDestinationRecord(
          Guid subscriptionId,
          Guid resourceId,
          DateTimeOffset dateTime,
          string meter,
          int amount,
          string reason,
          IDictionary<string, string> internalContext,
          IDictionary<string, string> subOwnerContext,
          string eventMessageTemplate)
        {
            var mockedDestination = new Mock<IReservedCapacityMetricsDestination>();

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
            var capturedDateTime = default(DateTimeOffset);
            var capturedMeter = default(string);
            var capturedAmount = default(int);
            var capturedReason = default(string);

            mockedDestination
                .Setup(d => d.ReserveCapacity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Callback((Guid callbackSubscriptionId, Guid callbackResourceId, DateTimeOffset callbackDateTime, string callbackMeter, int callbackAmount, string callbackReason) =>
                    (capturedSubscriptionId, capturedResourceId, capturedDateTime, capturedMeter, capturedAmount, capturedReason) =
                        (callbackSubscriptionId, callbackResourceId, callbackDateTime, callbackMeter, callbackAmount, callbackReason));

            mockedDestination
              .Setup(d => d.ReleaseCapacity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
              .Callback((Guid callbackSubscriptionId, Guid callbackResourceId, DateTimeOffset callbackDateTime, string callbackMeter, int callbackAmount, string callbackReason) =>
                  (capturedSubscriptionId, capturedResourceId, capturedDateTime, capturedMeter, capturedAmount, capturedReason) =
                    (callbackSubscriptionId, callbackResourceId, callbackDateTime, callbackMeter, callbackAmount, callbackReason));

            IReservedCapacityMetrics reservedCapacity = new ReservedCapacityMetrics(mockedDestination.Object);

            // Act
            reservedCapacity = internalContext?.Aggregate(reservedCapacity, (client, kvp) => client.ForInternalContext(kvp.Key, kvp.Value)) ?? reservedCapacity;
            reservedCapacity = subOwnerContext?.Aggregate(reservedCapacity, (client, kvp) => client.ForSubscriptionOwnerContext(kvp.Key, kvp.Value)) ?? reservedCapacity;

            if (eventMessageTemplate.StartsWith("Increased", StringComparison.InvariantCultureIgnoreCase))
            {
                reservedCapacity.Increase(subscriptionId, resourceId, dateTime, meter, amount, reason);
            }
            else if (eventMessageTemplate.StartsWith("Decreased", StringComparison.InvariantCultureIgnoreCase))
            {
                reservedCapacity.Decrease(subscriptionId, resourceId, dateTime, meter, amount, reason);
            }
            else
            {
                throw new Exception($"Unhandled message template: {eventMessageTemplate}");
            }

            return new ReserveOrReleaseCapacityRecord(
                eventMessageTemplate: eventMessageTemplate,
                subscriptionId: capturedSubscriptionId,
                resourceId: capturedResourceId,
                dateTime: capturedDateTime,
                meter: capturedMeter,
                amount: capturedAmount,
                reason: capturedReason,
                internalContext: capturedInternalContext,
                subscriptionOwnerContext: capturedSubOwnerContext);
        }

        [Theory]
        [InlineData(0, "Increased reserved capacity by {Amount} for {Meter} at {IncreaseDateTime} on resource {ResourceId} in subscription {SubscriptionId}")]
        [InlineData(0, "Decreased reserved capacity by {Amount} for {Meter} at {IncreaseDateTime} on resource {ResourceId} in subscription {SubscriptionId}")]
        [InlineData(-1, "Increased reserved capacity by {Amount} for {Meter} at {IncreaseDateTime} on resource {ResourceId} in subscription {SubscriptionId}")]
        [InlineData(-1, "Decreased reserved capacity by {Amount} for {Meter} at {IncreaseDateTime} on resource {ResourceId} in subscription {SubscriptionId}")]
        public void IncreaseOrDecreaseByNonPositiveThrowsArgumentOutOfRangeException(int amount, string messageTemplate)
        {
            throw new NotImplementedException();
        }

        [Theory]
        [InlineData("Increased reserved capacity by {Amount} for {Meter} at {IncreaseDateTime} on resource {ResourceId} in subscription {SubscriptionId}")]
        [InlineData("Decreased reserved capacity by {Amount} for {Meter} at {IncreaseDateTime} on resource {ResourceId} in subscription {SubscriptionId}")]
        public void IncreaseOrDecreaseReservedCapacityMetricsAllValues(string eventMessageTemplate)
        {
            // Arrange
            var subscriptionId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var meter = "Audit/Instance/Capacity";
            var amount = 1;
            var reason = "Any old reason will do";
            var internalContext = new Dictionary<string, string> { { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };
            var subOwnerContext = new Dictionary<string, string> { { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };
            var dateTime = DateTimeOffset.Now;

            // Act
            var result = CaptureDestinationRecord(
                subscriptionId: subscriptionId,
                resourceId: resourceId,
                dateTime: dateTime,
                meter: meter,
                amount: amount,
                reason: reason,
                internalContext: internalContext,
                subOwnerContext: subOwnerContext,
                eventMessageTemplate: eventMessageTemplate);

            // Assert
            result.Should().BeEquivalentTo(
                new ReserveOrReleaseCapacityRecord(
                    eventMessageTemplate,
                    subscriptionId,
                    resourceId,
                    dateTime,
                    meter,
                    amount,
                    reason,
                    internalContext,
                    subOwnerContext));
        }
    }
}
