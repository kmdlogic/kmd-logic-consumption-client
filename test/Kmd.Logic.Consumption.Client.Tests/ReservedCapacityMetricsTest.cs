using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ReservedCapacityMetricsTest
    {
        public static ReservedCapacityDestinationRecord CaptureDestinationRecord(
          Guid subscriptionId,
          Guid resourceId,
          DateTimeOffset reservedCapacityDateTime,
          string meter,
          int amount,
          string reason,
          IDictionary<string, string> internalContext,
          IDictionary<string, string> subOwnerContext,
          bool isIncreaseReservedCapacity)
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

            var reservedCapacitySubscriptionId = default(Guid);
            var reservedCapacityResourceId = default(Guid);
            var reservedCapacityMeter = default(string);
            var reservedCapacityAmount = default(int);
            var reservedCapacityReason = default(string);

            mockedDestination
                .Setup(d => d.ReserveCapacity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Callback((Guid callbackSubscriptionId, Guid callbackResourceId, string callbackMeter, int callbackAmount, string callbackReason) =>
                    (reservedCapacitySubscriptionId, reservedCapacityResourceId, reservedCapacityMeter, reservedCapacityAmount, reservedCapacityReason) =
                        (callbackSubscriptionId, callbackResourceId, callbackMeter, callbackAmount, callbackReason));

            mockedDestination
              .Setup(d => d.ReleaseCapacity(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
              .Callback((Guid callbackSubscriptionId, Guid callbackResourceId, string callbackMeter, int callbackAmount, string callbackReason) =>
                  (reservedCapacitySubscriptionId, reservedCapacityResourceId, reservedCapacityMeter, reservedCapacityAmount, reservedCapacityReason) =
                      (callbackSubscriptionId, callbackResourceId, callbackMeter, callbackAmount, callbackReason));

            IReservedCapacityMetrics reservedCapacity = new ReservedCapacityMetrics(mockedDestination.Object);
            reservedCapacity = internalContext?.Aggregate(reservedCapacity, (client, kvp) => client.ForInternalContext(kvp.Key, kvp.Value)) ?? reservedCapacity;
            reservedCapacity = subOwnerContext?.Aggregate(reservedCapacity, (client, kvp) => client.ForSubscriptionOwnerContext(kvp.Key, kvp.Value)) ?? reservedCapacity;

            if (isIncreaseReservedCapacity)
            {
                reservedCapacity.Increase(subscriptionId, resourceId, reservedCapacityDateTime, meter, amount, reason);
            }
            else
            {
                reservedCapacity.Decrease(subscriptionId, resourceId, reservedCapacityDateTime, meter, amount, reason);
            }

            return new ReservedCapacityDestinationRecord(
                subscriptionId: reservedCapacitySubscriptionId,
                resourceId: reservedCapacityResourceId,
                meter: reservedCapacityMeter,
                amount: reservedCapacityAmount,
                reason: reservedCapacityReason,
                internalContext: capturedInternalContext,
                subscriptionOwnerContext: capturedSubOwnerContext);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IncreaseOrDecreaseReservedCapacityMetricsAllValues(bool isIncreaseReservedCapacity)
        {
            // Arrange
            var subscriptionId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var meter = "Audit/Instance/Capacity";
            var amount = 1;
            var reason = "Any old reason will do";

            var internalContext = new Dictionary<string, string> { { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };
            var subOwnerContext = new Dictionary<string, string> { { $"{Guid.NewGuid()}", $"{Guid.NewGuid()}" } };
            DateTimeOffset reservedCapacityDateTime = DateTimeOffset.Now;

            // Act
            var result = CaptureDestinationRecord(
                    subscriptionId: subscriptionId,
                    resourceId: resourceId,
                    reservedCapacityDateTime: reservedCapacityDateTime,
                    meter: meter,
                    amount: amount,
                    reason: reason,
                    internalContext: internalContext,
                    subOwnerContext: subOwnerContext,
                    isIncreaseReservedCapacity);

            // Assert
            // Add IncreaseDateTime and DecreaseDateTime value in SubOwnerContext to verify these values are added.
            if (isIncreaseReservedCapacity)
            {
                subOwnerContext.Add("IncreaseDateTime", reservedCapacityDateTime.UtcTicks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                subOwnerContext.Add("DecreaseDateTime", reservedCapacityDateTime.UtcTicks.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            result.Should().BeEquivalentTo(
                new ReservedCapacityDestinationRecord(
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
