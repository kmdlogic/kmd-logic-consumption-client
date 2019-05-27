namespace Kmd.Logic.Consumption.Client
{
    using System;

    public interface IConsumptionMetricsDestination
    {
        IConsumptionMetricsDestination ForInternalContext(string propertyName, string value);

        IConsumptionMetricsDestination ForSubscriptionOwnerContext(string propertyName, string value);

        void Write(Guid subscriptionId, Guid resourceId, string meter, int amount, string reason = null);
    }
}
