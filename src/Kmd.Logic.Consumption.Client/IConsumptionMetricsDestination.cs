namespace Kmd.Logic.Consumption.Client
{
    public interface IConsumptionMetricsDestination
    {
        IConsumptionMetricsDestination ForContext(string propertyName, string value);

        void Write(string messageTemplate, params object[] propertyValues);
    }
}
