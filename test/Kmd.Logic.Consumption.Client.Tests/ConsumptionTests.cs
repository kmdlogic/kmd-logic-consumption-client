using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ConsumptionTests
    {
        [Fact]
        public void ConsumptionTestsExists()
        {
            Assert.NotNull(typeof(IConsumptionMetrics));
        }
    }
}
