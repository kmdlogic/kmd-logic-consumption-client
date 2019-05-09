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

        public IConsumptionMetricsDestination ForContext(string propertyName, string value)
        {
            return new AuditClientConsumptionMetricsDestination(audit.ForContext(propertyName, value));
        }

        public void Write(string messageTemplate, params object[] propertyValues)
        {
            this.audit.Write(messageTemplate, propertyValues);
        }        
    }
}
