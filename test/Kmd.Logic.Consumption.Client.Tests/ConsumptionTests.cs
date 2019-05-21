using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ConsumptionTests
    {
        [Fact]
        public void ConsumptionMetricsTestsExists()
        {
            Assert.NotNull(typeof(IConsumptionMetrics));
        }

        [Fact]
        public void ConsumptionMetricsDestinationExists()
        {
            Assert.NotNull(typeof(IConsumptionMetricsDestination));
        }
    }
}
