using System;
using System.Collections.Generic;
using System.Text;

namespace Kmd.Logic.Consumption.Client
{
    public class ReservedCapacityMetrics : IReservedCapacityMetrics
    {
        private readonly IReservedCapacityMetricsDestination _destination;

        public ReservedCapacityMetrics(IReservedCapacityMetricsDestination destination)
        {
            this._destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }

        public void Increase(Guid subscriptionId, Guid resourceId, DateTimeOffset dateTime, string meter, int amount, string reason = null)
        {
            this._destination.ReserveCapacity(
                subscriptionId: subscriptionId,
                resourceId: resourceId,
                dateTime: dateTime,
                meter: meter,
                amount: amount,
                reason: reason);
        }

        public void Decrease(Guid subscriptionId, Guid resourceId, DateTimeOffset dateTime, string meter, int amount, string reason = null)
        {
            this._destination.ReleaseCapacity(
                subscriptionId: subscriptionId,
                resourceId: resourceId,
                dateTime: dateTime,
                meter: meter,
                amount: amount,
                reason: reason);
        }

        public IReservedCapacityMetrics ForInternalContext(string name, string value)
        {
            return new ReservedCapacityMetrics(this._destination
               .ForInternalContext(
                   propertyName: name,
                   value: value));
        }

        public IReservedCapacityMetrics ForSubscriptionOwnerContext(string name, string value)
        {
            return new ReservedCapacityMetrics(this._destination
               .ForSubscriptionOwnerContext(
                   propertyName: name,
                   value: value));
        }
    }
}
