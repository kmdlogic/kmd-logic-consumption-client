namespace Kmd.Logic.Consumption.Client
{
    public interface IConsumptionMetricsDestination
    {
        IConsumptionMetricsDestination ForInternalContext(string propertyName, string value);

        IConsumptionMetricsDestination ForSubscriptionOwnerContext(string propertyName, string value);

        void Write(string messageTemplate, params object[] propertyValues);
    }
}
