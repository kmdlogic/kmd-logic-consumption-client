using System;
using System.Collections.Generic;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ReserveOrReleaseCapacityRecord
    {
        public ReserveOrReleaseCapacityRecord(
            string eventMessageTemplate,
            Guid subscriptionId,
            Guid resourceId,
            DateTimeOffset dateTime,
            string meter,
            int amount,
            string reason,
            IDictionary<string, string> internalContext,
            IDictionary<string, string> subscriptionOwnerContext)
        {
            this.EventMessageTemplate = eventMessageTemplate;
            this.SubscriptionId = subscriptionId;
            this.ResourceId = resourceId;
            this.DateTime = dateTime;
            this.Meter = meter;
            this.Amount = amount;
            this.Reason = reason;
            this.InternalContext = internalContext;
            this.SubscriptionOwnerContext = subscriptionOwnerContext;
        }

        public string EventMessageTemplate { get; }

        public Guid SubscriptionId { get; }

        public Guid ResourceId { get; }

        public DateTimeOffset DateTime { get; }

        public string Meter { get; }

        public int Amount { get; }

        public string Reason { get; }

        public IDictionary<string, string> InternalContext { get; }

        public IDictionary<string, string> SubscriptionOwnerContext { get; }
    }
}