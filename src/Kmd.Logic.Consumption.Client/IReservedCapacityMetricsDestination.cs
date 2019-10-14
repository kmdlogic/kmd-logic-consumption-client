using System;

namespace Kmd.Logic.Consumption.Client
{
    public interface IReservedCapacityMetricsDestination
    {
        IReservedCapacityMetricsDestination ForInternalContext(string propertyName, string value);

        IReservedCapacityMetricsDestination ForSubscriptionOwnerContext(string propertyName, string value);

        /// <summary>
        /// This is adding more capacity to the resource.
        /// </summary>
        /// <param name="subscriptionId">subscriptionId is a Guid created by the subscription management.</param>
        /// <param name="resourceId">ResourceId is a Guid created by the resource registration.</param>
        /// <param name="dateTime">The date/time when the capacity was reserved (increased).</param>
        /// <param name="meter">meter is defined by the resource type. For example, an "Audit Instance" might decide a meter is "Audit/Instance/Capacity".</param>
        /// <param name="amount">Amount is the scale of the resource. Usually this is "1". It must be a positive number.</param>
        /// <param name="reason">Optional, specify reason if any.</param>
        void ReserveCapacity(Guid subscriptionId, Guid resourceId, DateTimeOffset dateTime, string meter, int amount, string reason = null);

        /// <summary>
        /// This is removing capacity from the resource.
        /// </summary>
        /// <param name="subscriptionId">subscriptionId is a Guid created by the subscription management.</param>
        /// <param name="resourceId">ResourceId is a Guid created by the resource registration.</param>
        /// <param name="dateTime">The date/time when the capacity was released (decreased).</param>
        /// <param name="meter">meter is defined by the resource type. For example, an "Audit Instance" might decide a meter is "Audit/Instance/Capacity".</param>
        /// <param name="amount">Amount is the scale of the resource. Usually this is "1". It must be a positive number.</param>
        /// <param name="reason">Optional, specify reason if any.</param>
        void ReleaseCapacity(Guid subscriptionId, Guid resourceId, DateTimeOffset dateTime, string meter, int amount, string reason = null);
    }
}
