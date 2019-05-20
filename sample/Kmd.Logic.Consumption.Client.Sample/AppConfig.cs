namespace Kmd.Logic.Consumption.Client.Sample
{
    internal class AppConfig
    {
        public string EventSource { get; set; } = null;

        public int NumberOfEvents { get; set; } = 10;

        public int NumberOfThreads { get; set; } = 1;

        public string EventHubsConnectionString { get; set; }

        public AppConfigConsumptionData ConsumptionData { get; set; } = new AppConfigConsumptionData();
    }
}