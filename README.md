# KMD Logic Consumption Client

This is a dotnet client library for KMD Logic billing, which allows applications to record consumption metrics reliably and securely.

The KMD Logic Consumption client utilises many modern concepts from [Serilog](https://serilog.net/) and [Seq](https://getseq.net/), such as [Message Templates](https://messagetemplates.org/) and ingestion endpoints capable of understanding [CLEF](https://docs.getseq.net/docs/posting-raw-events).

## How to use this client library

In projects or components where you need to *generate* consumption metrics, add a NuGet package reference to [Kmd.Logic.Consumption.Client](https://www.nuget.org/packages/Kmd.Logic.Consumption.Client), and use the `IConsumptionMetrics` interface like this:

```csharp
consumptionClient
    .ForSubscriptionOwnerContext("ReportableField", "Anything")
    .ForInternalContext("EventNumber", "${eventNumber}")
    .Record(
        subscriptionId: subscriptionId,
        resourceId: resourceId,
        meter: "SMS/BYO/Send SMS",
        amount: 1,
        reason: "Just testing");
```

 * Add **Kmd.Logic.Consumption.Client.AuditClient** library from nuget package [Consumption Audit Client](https://www.nuget.org/packages/Kmd.Logic.Consumption.Client.AuditClient/) to consume Audit client destination which is implemented `IConsumptionMetricsDestination`.

The container(destination) will vary based on customer's implementation . Here we are referring the container(destination)  **kmd.logic.audit.client.serilogazureeventhubs**. This library can be downloaded from [Serilog Azure Eventhubs](https://www.nuget.org/packages/Kmd.Logic.Audit.Client.SerilogAzureEventHubs/). Consumer can change the exiting destination by using defined destination. Example as in below:
```csharp
var auditClient = new SerilogAzureEventHubsAuditClient(clientConfig);            
var auditConsumptionDestination = new AuditClientConsumptionMetricsDestination(auditClient);
            
```


> NOTE: We have implemented this functionality initially by reusing [Serilog](https://github.com/serilog/serilog), the [Seq sink](https://github.com/serilog/serilog-sinks-seq) and the [KMDLogic audit](https://github.com/kmdlogic/kmd-logic-audit-client). We intend to publish a version of this client library in the future that has no such external dependencies. If this issue impacts you negatively, please let us know.

## How to contribute

1. Fork the project & clone locally.
2. Create an upstream remote and sync your local copy before you branch.
3. Branch for each separate piece of work.
4. Do the work, write good commit messages, and read the CONTRIBUTING file if there is one.
5. Push to your origin repository.
6. Create a new PR in GitHub.

## Contact us

Contact us at discover@kmdlogic.io for more information.