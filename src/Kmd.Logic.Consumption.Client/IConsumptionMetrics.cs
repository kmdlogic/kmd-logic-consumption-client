using System;

namespace Kmd.Logic.Consumption.Client
{
    public interface IConsumptionMetrics
    {
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
        /// Adds arbitrary property names and values (context) to the consumption metrics record.
        /// </summary>
        /// <remarks>
        /// Each consumption event may need additional contextual information attached to it in order
        /// for reporting or future understanding by humans or machines reading the event. For example,
        /// if a "backup" was performed ("consumed") then we might need to know which system the backup
        /// was taken from in order to correctly understand what the consumption applied to. If you simply
        /// said "a backup was taken" without the additional context, it might be impossible to understand
        /// why that entry was made in the future, and perhaps difficult or impossible to reconcile the
        /// consumption with other records.
        /// </remarks>
        IConsumptionMetrics ForContext(string name, string value);

        /// <summary>
        /// Adds arbitrary property names and values (context) to the consumption metrics record, and 
        /// marks those properties as being necessary for a generated consumption report. 
        /// </summary>
        /// <remarks>
        /// The same as <see cref="ForContext" /> however any contextual properties added via this 
        /// method will be visible by humans who read the generated consumption reports.
        /// </remarks>
        IConsumptionMetrics ForContextReport(string name, string value);
    }
}