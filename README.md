# KMD Logic Consumption Client

A dotnet client library for the KMD Logic billing, which allows applications to record consumption metrics reliably and securely.

The KMD Logic Consumption service utilises many modern concepts from [Serilog](https://serilog.net/) and [Seq](https://getseq.net/), such as [Message Templates](https://messagetemplates.org/) and ingestion endpoints capable of understanding [CLEF](https://docs.getseq.net/docs/posting-raw-events).

## How to use this client library

In projects or components where you need to *write* consumption events, add a NuGet package reference to [Kmd.Logic.Consumption.Client](https://www.nuget.org/packages/Kmd.Logic.Audit.Client), and use the `IConsumptionMetrics` interface like this:

```csharp
consumption
            .ForContext("ResourceType", resourceType)
            .ForContext("ResourceName", resourceName)
            .Record(
              subscriptionId: {subscriptionId},
              resourceId: {resourceId},
              consumptionType: {consumptionType},
              consumptionAmount: +1);
```

In your applications `Main()`, or `Startup.ConfigureServices()` or other [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/), create a singleton instance of `**Kmd.Logic.Consumption.Client**` and use it as the implementation of `IConsumptionMetrics` and `IConsumptionMetricsDestination` by injecting it into your container or exposing it as a static property or method. Since `**SerilogAzureEventHubsAuditClient**` is thread-safe and requires disposal, it would be appropriate to use a singleton lifetime in a DI container that will dispose of it upon application shut down.

To demonstrate this without a DI container:

```csharp
 using (var serilogAzureEventHubsAuditClient = new SerilogAzureEventHubsAuditClient(clientConfig))
            {
                IConsumptionMetricsDestination consumptionDestination = new AuditClientConsumptionMetricsDestination(serilogAzureEventHubsAuditClient);
                IConsumptionMetrics consumption = new ConsumptionClient(consumptionDestination);
                Parallel.For(0, 10, t =>
                {
                    consumption.ForContext("ResourceType", resourceType)
                            .ForContext("ResourceName", resourceName)
                            .Record(
                                subscriptionId: subscriptionId,
                                resourceId: resourceId,
                                consumptionType: consumptionType,
                                consumptionAmount: +10);
                });
            }

```

> NOTE: We have implemented this functionality initially by  reusing [Serilog](https://github.com/serilog/serilog), the [Seq sink](https://github.com/serilog/serilog-sinks-seq) and the [KMDLogic audit](https://github.com/kmdlogic/kmd-logic-audit-client). We intend to publish a version of this client library in the future that has no external dependencies. If this issue impacts you negatively, please let us know.
Package in components that write events, and depend on the [Kmd.Logic.Audit.Client.SerilogAzureEventHubs](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogAzureEventHubs/1.2.2) package in your application composition root only.

## How to contribute

Contact us at discover@kmdlogic.io for more information.
