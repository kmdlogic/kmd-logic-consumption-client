using System;
using System.Collections.Generic;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ConsumptionMetricsDestinationRecord
    {
        public ConsumptionMetricsDestinationRecord(
            Guid subscriptionId,
            Guid resourceId,
            string meter,
            int amount,
            DateTimeOffset consumedDateTime,
            string reason,
            IDictionary<string, string> internalContext,
            IDictionary<string, string> subscriptionOwnerContext)
        {
            this.SubscriptionId = subscriptionId;
            this.ResourceId = resourceId;
            this.Meter = meter;
            this.Amount = amount;
            this.ConsumedDateTime = consumedDateTime;
            this.Reason = reason;
            this.InternalContext = internalContext;
            this.SubscriptionOwnerContext = subscriptionOwnerContext;
        }

        public Guid SubscriptionId { get; }

        public Guid ResourceId { get; }

        public string Meter { get; }

        public int Amount { get; }

        public DateTimeOffset ConsumedDateTime { get; }

        public string Reason { get; }

        public IDictionary<string, string> InternalContext { get; }

        public IDictionary<string, string> SubscriptionOwnerContext { get; }
    }
}