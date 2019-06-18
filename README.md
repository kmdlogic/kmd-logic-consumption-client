# KMD Logic Consumption Client

A dotnet client library for the KMD Logic billing, which allows applications to record consumption metrics reliably and securely.

The KMD Logic Consumption client utilises many modern concepts from [Serilog](https://serilog.net/) and [Seq](https://getseq.net/), such as [Message Templates](https://messagetemplates.org/) and ingestion endpoints capable of understanding [CLEF](https://docs.getseq.net/docs/posting-raw-events).

## How to use this client library

In projects or components where you need to *generate* consumption metrics, add a NuGet package reference to [Kmd.Logic.Consumption.Client](https://www.nuget.org/packages/Kmd.Logic.Consumption.Client).
 
Use the `IConsumptionMetrics` interface like this:

```c#
consumptionClient
    .ForSubscriptionOwnerContext("ReportableField", "Anything")
    .ForInternalContext("EventNumber", $"{eventNumber}")
    .Record(
        subscriptionId: subscriptionId,
        resourceId: resourceId,
        meter: "SMS/BYO/Send SMS",
        amount: 1,
        reason: "Just testing");
```

Use the `IReservedCapacityMetrics` interface like this:

```c#
var contextReservedCapacity = reservedCapaty
    .ForSubscriptionOwnerContext("ReportableField", "Anything")
    .ForInternalContext("EventNumber", $"{eventNumber}");

// Increase - Reserve capacity
contextReservedCapacity
    .Increase(
        subscriptionId: subscriptionId,
        resourceId: resourceId,
        meter: "Audit/Instance/Capacity",
        amount: 1,
        dateTime: DateTimeOffset.Now);

// Decrease - release capacity
contextReservedCapacity
    .Decrease(
        subscriptionId: subscriptionId,
        resourceId: resourceId,
        meter: "Audit/Instance/Capacity",
        amount: 1,
        dateTime: DateTimeOffset.Now);
```

TODO
1. explain how to setup and configure a container for `AuditClientConsumptionMetricsDestination`
2. explain how to setup and configure a container for `AuditClientReservedCapacityMetricsDestination`

> NOTE: We have implemented this functionality initially by reusing [Serilog](https://github.com/serilog/serilog), the [Seq sink](https://github.com/serilog/serilog-sinks-seq) and the [KMD Logic Audit Client](https://github.com/kmdlogic/kmd-logic-audit-client). We intend to publish a version of this client library in the future that has no such external dependencies. If this issue impacts you negatively, please let us know so that we can prioritise appropriately.

## How to contribute

TODO

## Contact us

Contact us at discover@kmdlogic.io for more information.
