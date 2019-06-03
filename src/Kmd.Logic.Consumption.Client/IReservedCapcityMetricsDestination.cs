using System;

namespace Kmd.Logic.Consumption.Client
{
    public interface IReservedCapcityMetricsDestination
    {
        IReservedCapcityMetricsDestination ForInternalContext(string propertyName, string value);

        IReservedCapcityMetricsDestination ForSubscriptionOwnerContext(string propertyName, string value);

        /// <summary>
        /// This is adding more capacity to the resource.
        /// </summary>
        /// <param name="subscriptionId">subscriptionId is a Guid created by the subscription management.</param>
        /// <param name="resourceId">ResourceId is a Guid created by the resource registration.</param>
        /// <param name="meter">meter is defined by the resource type. For example, an "Audit Instance" might decide a meter is "Audit/Instance/Capacity".</param>
        /// <param name="amount">Amount is scale of the resource.</param>
        /// <param name="reason">Optional, specify reason if any.</param>
        void ReserveCapacity(Guid subscriptionId, Guid resourceId, string meter, int amount, string reason = null);

        /// <summary>
        /// This is removing capacity from the resource.
        /// </summary>
        /// <param name="subscriptionId">subscriptionId is a Guid created by the subscription management.</param>
        /// <param name="resourceId">ResourceId is a Guid created by the resource registration.</param>
        /// <param name="meter">meter is defined by the resource type. For example, an "Audit Instance" might decide a meter is "Audit/Instance/Capacity".</param>
        /// <param name="amount">Amount is scale of the resource.</param>
        /// <param name="reason">Optional, specify reason if any.</param>
        void ReleaseCapacity(Guid subscriptionId, Guid resourceId, string meter, int amount, string reason = null);
    }
}
