0.5.0
* Bumped verison to avoid package collision from multiple build accounts.

0.4.0
* Allow to record event with date and time.

0.3.0
* Added the ability to record reserved capacity. Use this when a product/service is billed by an amount of capacity reserved for a period of time. Call the `IReservedCapacityMetrics.Increase(...)` method to record that some amount of capacity was reserved, then call `IReservedCapacityMetrics.Decrease(...)` to record that some reserved capacity was released. 

0.2.0
* Fixed `resourceId` and `subscriptionId` being in the wrong message template positions
* Improved `IConsumptionMetricsDestination` so we don't require message templates

0.1.0
* Initial version
