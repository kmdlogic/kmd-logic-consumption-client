using System;

namespace Kmd.Logic.Consumption.Client
{
    public interface IConsumptionMetrics
    {
        void Record(Guid subscriptionId, Guid resourceId, string consumptionType, int consumptionAmount, string reason = null);

        IConsumptionMetrics ForContext(string name, string value);
    }
}