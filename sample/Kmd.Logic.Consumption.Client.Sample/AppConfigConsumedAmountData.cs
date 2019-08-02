using System;

namespace Kmd.Logic.Consumption.Client.Sample
{
    internal class AppConfigConsumedAmountData
    {
        public string ResourceType { get; set; } = "SMS Provider";

        public string ResourceName { get; set; } = "FRIE PROD";

        public int Amount { get; set; } = 1;

        public DateTimeOffset ConsumedDateTime { get; set; } = DateTimeOffset.Now;

        public string Reason { get; set; } = "Just testing";
    }
}