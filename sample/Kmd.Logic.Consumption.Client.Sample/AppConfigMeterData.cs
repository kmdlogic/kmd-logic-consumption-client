using System;

namespace Kmd.Logic.Consumption.Client.Sample
{
    internal class AppConfigMeterData
    {
        public string Meter { get; set; } = "SMS/BYO/Send SMS";

        public Guid SubscriptionId { get; set; } = Guid.NewGuid();

        public Guid ResourceId { get; set; } = Guid.NewGuid();
    }
}