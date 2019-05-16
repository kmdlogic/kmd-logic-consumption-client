using System;

namespace Kmd.Logic.Consumption.Client.Sample
{
    internal class AppConfigConsumptionData
    {
        public string Meter { get; set; } = "SMS/BYO/Send SMS";

        public Guid SubscriptionId { get; set; } = Guid.NewGuid();

        public Guid ResourceId { get; set; } = Guid.NewGuid();

        public string ResourceType { get; set; } = "SMS Provider";

        public string ResourceName { get; set; } = "FRIE PROD";

        public int Amount { get; set; } = 1;

        public string Reason { get; set; } = "Just testing";
    }
}