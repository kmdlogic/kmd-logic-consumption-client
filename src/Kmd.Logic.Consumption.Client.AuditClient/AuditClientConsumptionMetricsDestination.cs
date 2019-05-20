using System;
using Kmd.Logic.Audit.Client;

namespace Kmd.Logic.Consumption.Client.AuditClient
{
    public class AuditClientConsumptionMetricsDestination : IConsumptionMetricsDestination
    {
        private readonly IAudit audit;

        public AuditClientConsumptionMetricsDestination(IAudit audit)
        {
            this.audit = audit ?? throw new ArgumentNullException(nameof(audit));
        }

        public IConsumptionMetricsDestination ForInternalContext(string propertyName, string value)
        {
            return new AuditClientConsumptionMetricsDestination(this.audit.ForContext(propertyName, value));
        }

        public IConsumptionMetricsDestination ForSubscriptionOwnerContext(string propertyName, string value)
        {
            return new AuditClientConsumptionMetricsDestination(
                this.audit.ForContext($"__Sub_{propertyName}", value));
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.audit.Write(messageTemplate, propertyValues);
        }
    }
}
