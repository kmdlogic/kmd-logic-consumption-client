using System.Collections.Generic;

namespace Kmd.Logic.Consumption.Client.Tests
{
    public class ConsumptionMetricsDestinationRecord
    {
        public ConsumptionMetricsDestinationRecord(
            string messageTemplate,
            object[] args,
            IDictionary<string, string> internalContext,
            IDictionary<string, string> subscriptionOwnerContext)
        {
            this.MessageTemplate = messageTemplate;
            this.MessageTemplateArgs = args;
            this.InternalContext = internalContext;
            this.SubscriptionOwnerContext = subscriptionOwnerContext;
        }

        public string MessageTemplate { get; }

        private object[] MessageTemplateArgs { get; }

        public object[] GetMessageTemplateArgs()
        {
            return this.MessageTemplateArgs;
        }

        public IDictionary<string, string> InternalContext { get; }

        public IDictionary<string, string> SubscriptionOwnerContext { get; }
    }
}