using System;

namespace Kmd.Logic.Consumption.Client
{
    public class ConsumptionMetrics : IConsumptionMetrics
    {
        private readonly IConsumptionMetricsDestination _destination;

        public ConsumptionMetrics(IConsumptionMetricsDestination destination)
        {
            this._destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }

        public IConsumptionMetrics ForInternalContext(string name, string value)
        {
            return new ConsumptionMetrics(this._destination
                .ForInternalContext(
                    propertyName: name,
                    value: value));
        }

        public IConsumptionMetrics ForSubscriptionOwnerContext(string name, string value)
        {
            return new ConsumptionMetrics(this._destination
                .ForSubscriptionOwnerContext(
                    propertyName: name,
                    value: value));
        }

        public void Record(Guid subscriptionId, Guid resourceId, string meter, int amount, DateTimeOffset consumedDateTime, string reason = null)
        {
            this._destination.Write(subscriptionId, resourceId, meter, amount, consumedDateTime, reason);
        }
    }
}
