using System;
using Kmd.Logic.Audit.Client;

namespace Kmd.Logic.Consumption.Client.AuditClient
{
    public class AuditClientConsumptionMetricsDestination : IConsumptionMetricsDestination
    {
        public static string Template { get; } = "Consumed {Amount} for {Meter} on resource {ResourceId} at {ConsumedDatetime} in subscription {SubscriptionId}";

        public static string GetDefaultSubOwnerContextName(string propertyName) => $"__Sub_{propertyName}";

        private readonly IAudit _audit;
        private readonly Func<string, string> _getSubOwnerContextName;

        public AuditClientConsumptionMetricsDestination(IAudit audit, Func<string, string> getSubOwnerContextName = null)
        {
            this._audit = audit ?? throw new ArgumentNullException(nameof(audit));
            this._getSubOwnerContextName = getSubOwnerContextName ?? GetDefaultSubOwnerContextName;
        }

        public IConsumptionMetricsDestination ForInternalContext(string propertyName, string value)
        {
            return new AuditClientConsumptionMetricsDestination(
                this._audit.ForContext(propertyName, value),
                this._getSubOwnerContextName);
        }

        public IConsumptionMetricsDestination ForSubscriptionOwnerContext(string propertyName, string value)
        {
            return new AuditClientConsumptionMetricsDestination(
                this._audit.ForContext(this._getSubOwnerContextName(propertyName), value),
                this._getSubOwnerContextName);
        }

        public void Write(Guid subscriptionId, Guid resourceId, string meter, int amount, string reason = null)
        {
            this.Write(
                subscriptionId: subscriptionId,
                resourceId: resourceId,
                meter: meter,
                amount: amount,
                consumedDatetime: DateTimeOffset.Now,
                reason: reason);
        }

        public void Write(Guid subscriptionId, Guid resourceId, string meter, int amount, DateTimeOffset consumedDatetime, string reason = null)
        {
            var audit = reason == null
                       ? this._audit
                       : this._audit.ForContext(this._getSubOwnerContextName("Reason"), reason);

            audit = consumedDatetime == null
                      ? audit
                      : audit.ForContext(this._getSubOwnerContextName("consumedDatetime"), consumedDatetime);

            audit.Write(
                Template,
                amount,
                meter,
                resourceId,
                subscriptionId,
                consumedDatetime);
        }
    }
}