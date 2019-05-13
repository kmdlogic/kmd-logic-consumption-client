using System;

namespace Kmd.Logic.Consumption.Client
{
    public interface IConsumptionMetrics
    {
#pragma warning disable SA1611
        /// <summary>
        /// Record the consumption against the subscription and resource id using the specified
        /// consumption type (and optionally give a free text reason).
        /// </summary>
        /// <remarks>
        /// When a subscription user has consumed resources (e.g. sent an SMS or performed a backup),
        /// we call this method to record what was "consumed".
        /// </remarks>
        void Record(Guid subscriptionId, Guid resourceId, string consumptionType, int consumptionAmount, string reason = null);

        /// <summary>
        /// Adds arbitrary property names and values (context) to the consumption metrics record. Use this method
        /// to give additional context to internal (e.g. DevOps) teams regarding how and when the consumption happened.
        /// </summary>
        /// <remarks>
        /// Each consumption event may need additional contextual information attached to it in order
        /// for reporting or future understanding by humans or machines reading the event. For example,
        /// if a "backup" was performed ("consumed") then we might need to know which system ID the backup
        /// was taken from and which storage blob account it was copied to in order to correctly understand
        /// how the consumption happened. If you simply said "a backup was taken" without the additional context,
        /// it might be impossible to report or diagnose why that entry was made in the future, and perhaps
        /// difficult or impossible to reconcile the consumption with other records.
        /// </remarks>
        /// <returns>An instance with the additional context values included.</returns>
        IConsumptionMetrics ForInternalContext(string name, string value);

        /// <summary>
        /// Adds arbitrary property names and values (context) to the consumption metrics record. Use this method
        /// to give additional context to the subscription owners (i.e. the customer) regarding how and when the
        /// consumption happened.
        /// </summary>
        /// <remarks>
        /// The same as <see cref="ForInternalContext" /> however any contextual properties added via this
        /// method will be visible by the subscription owners too. Always use this to provide additional
        /// data to subsription owners so that they will understand how the consumption happened and what
        /// it applies to.
        /// </remarks>
        /// <returns>An instance with the additional context values included.</returns>
        IConsumptionMetrics ForSubscriptionOwnerContext(string name, string value);
#pragma warning restore SA1611
    }
}
