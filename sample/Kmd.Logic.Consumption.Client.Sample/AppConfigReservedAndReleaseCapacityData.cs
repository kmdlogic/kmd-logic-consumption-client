using System;

namespace Kmd.Logic.Consumption.Client.Sample
{
    public class AppConfigReservedAndReleaseCapacityData
    {
        public int CapacityAmount { get; set; } = 1;

        public DateTimeOffset ReservedDateTime { get; set; } = DateTimeOffset.Now.AddDays(-1);

        public DateTimeOffset ReleasedDateTime { get; set; } = DateTimeOffset.Now;
    }
}