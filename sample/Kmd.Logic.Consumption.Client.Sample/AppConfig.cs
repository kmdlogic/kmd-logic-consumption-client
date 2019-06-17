namespace Kmd.Logic.Consumption.Client.Sample
{
    internal class AppConfig
    {
        public string EventSource { get; set; } = null;

        public int NumberOfEvents { get; set; } = 1;

        public int NumberOfThreads { get; set; } = 1;

        public string EventHubsConnectionString { get; set; }

        public ConsumptionKind Kind { get; set; } = ConsumptionKind.ConsumedAmount;

        public AppConfigMeterData MeterData { get; set; } = new AppConfigMeterData();

        public AppConfigReservedAndReleaseCapacityData ReservedAndReleaseCapacityData { get; set; } = new AppConfigReservedAndReleaseCapacityData();

        public AppConfigConsumedAmountData ConsumedAmountData { get; set; } = new AppConfigConsumedAmountData();
    }
}