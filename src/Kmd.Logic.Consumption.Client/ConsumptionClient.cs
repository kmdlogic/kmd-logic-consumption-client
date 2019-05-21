using System;

namespace Kmd.Logic.Consumption.Client
{
    public class ConsumptionClient : IConsumptionMetrics
    {
        private readonly IConsumptionMetricsDestination destination;

        public ConsumptionClient(IConsumptionMetricsDestination destination)
        {
            this.destination = destination ?? throw new ArgumentNullException(nameof(destination));
        }

        public IConsumptionMetrics ForInternalContext(string name, string value)
        {
            return new ConsumptionClient(this.destination
                .ForInternalContext(
                    propertyName: name,
                    value: value));
        }

        public IConsumptionMetrics ForSubscriptionOwnerContext(string name, string value)
        {
            return new ConsumptionClient(this.destination
                .ForSubscriptionOwnerContext(
                    propertyName: name,
                    value: value));
        }

        public void Record(Guid subscriptionId, Guid resourceId, string meter, int amount, string reason = null)
        {
            var dest = this.destination;
            if (!string.IsNullOrEmpty(reason))
            {
                dest = dest.ForInternalContext("Reason", reason);
            }

            dest.Write("Consumed {Amount} for {Meter} on resource {ResourceId} in subscription {SubscriptionId}", amount, meter, resourceId, subscriptionId);
        }
    }
}
