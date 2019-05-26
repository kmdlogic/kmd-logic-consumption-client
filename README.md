# KMD Logic Consumption Client

This is a dotnet client library for KMD Logic which allows recording consumption metrics reliably and securely.

The KMD Logic Consumption client utilises modern concepts such as [Message Templates](https://messagetemplates.org/) and the [Compact Log Event Format (CLEF)](https://github.com/serilog/serilog-formatting-compact#format-details). To reliably record and ingest consumption metrics, we send events to an [Azure EventHub](https://docs.microsoft.com/en-us/azure/event-hubs/) leveraging the [Kmd.Logic.Audit.Client.SerilogAzureEventHubs](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogAzureEventHubs). 

## How to use this library

### Reference the `Kmd.Logic.Consumption.Client` NuGet package

In projects or components where you need to *generate* consumption metrics, add a NuGet package reference to [Kmd.Logic.Consumption.Client](https://www.nuget.org/packages/Kmd.Logic.Consumption.Client), and use the `IConsumptionMetrics` interface like this:

```csharp
consumption
    .ForSubscriptionOwnerContext("ReportableField", "Anything")
    .ForInternalContext("EventNumber", "${eventNumber}")
    .Record(
        subscriptionId: subscriptionId,
        resourceId: resourceId,
        meter: "SMS/BYO/Send SMS",
        amount: 1,
        reason: "Just testing");
```

### Choose your metrics destination

### Use the `Kmd.Logic.Audit.Client`

TODO

> NOTE: We have implemented this functionality initially by reusing [Serilog](https://github.com/serilog/serilog), the [Seq sink](https://github.com/serilog/serilog-sinks-seq) and the [KMDLogic audit](https://github.com/kmdlogic/kmd-logic-audit-client). We intend to publish a version of this client library in the future that has no such external dependencies. If this issue impacts you negatively, please let us know.

### Create your own backend

Consumption metrics can be delivered to any `IConsumptionMetricsDestination` implementation. The interface is defined as follows:

```csharp
public interface IConsumptionMetricsDestination
{
    IConsumptionMetricsDestination ForInternalContext(string propertyName, string value);
    IConsumptionMetricsDestination ForSubscriptionOwnerContext(string propertyName, string value);
    void Write(Guid subscriptionId, Guid resourceId, string meter, int amount, string reason = null);
}
```

Calling `ForInternalContext` or `ForSubscriptionOwnerContext` will return a new instance that, when `Write` is called, will include those properties in the written metrics event.

## Contact us

Contact us at discover@kmdlogic.io for more information.