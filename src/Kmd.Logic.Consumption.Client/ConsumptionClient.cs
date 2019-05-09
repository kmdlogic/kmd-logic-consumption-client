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

        public IConsumptionMetrics ForContext(string name, string value)
        {
            return new ConsumptionClient(this.destination
                .ForContext(
                    propertyName: name,
                    value: value));
        }

        public void Record(Guid subscriptionId, Guid resourceId, string consumptionType, int consumptionAmount, string reason = null)
        {
            this.destination
                .Write("Consumed {ConsumptionAmount} for {ConsumptionType} on resource {ResourceId} in subscription {SubscriptionId}", consumptionAmount, consumptionType, subscriptionId, resourceId);
        }
    }
}
