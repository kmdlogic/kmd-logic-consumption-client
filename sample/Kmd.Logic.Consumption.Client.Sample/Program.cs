using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Logic.Audit.Client;
using Kmd.Logic.Audit.Client.SerilogAzureEventHubs;
using Kmd.Logic.Consumption.Client;
using Kmd.Logic.Consumption.Client.AuditClient;
using Microsoft.Extensions.Configuration;
using Serilog;
using SerilogTimings;

namespace Kmd.Logic.Consumption.Client.Sample
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var configSource = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>())
                .AddCommandLine(args)
                .Build();

            var config = configSource.Get<AppConfig>();

            if (string.IsNullOrEmpty(config?.EventHubsConnectionString))
            {
                Log.Fatal("Please provide the connection string argument. E.g. {Args}", "--EventHubsConnectionString=\"XXX\"");
                return;
            }

            var clientConfig = new SerilogAzureEventHubsAuditClientConfiguration
            {
                ConnectionString = config.EventHubsConnectionString,
                EventSource = config.EventSource ?? $"Consumption client sample on {Environment.MachineName}",
            };
            var eventHubsTopic = clientConfig.AuditEventTopic;
            var eventHubsHost = new Microsoft.Azure.EventHubs.EventHubsConnectionStringBuilder(clientConfig.ConnectionString).Endpoint;

            using (var auditClient = new SerilogAzureEventHubsAuditClient(clientConfig))
            {
                switch (config.Kind)
                {
                    case ConsumptionKind.ReserveAndReleaseCapacity:
                        RecordReservedAndReleaseCapacity(
                            auditClient,
                            numberOfEvents: config.NumberOfEvents,
                            numberOfThreads: config.NumberOfThreads,
                            eventHubsTopic: eventHubsTopic,
                            eventHubsHost: $"{eventHubsHost}",
                            meterData: config.MeterData,
                            data: config.ReservedAndReleaseCapacityData);
                        break;
                    case ConsumptionKind.ConsumedAmount:
                        RecordConsumedAmount(
                            auditClient,
                            numberOfEvents: config.NumberOfEvents,
                            numberOfThreads: config.NumberOfThreads,
                            eventHubsTopic: eventHubsTopic,
                            eventHubsHost: $"{eventHubsHost}",
                            meterData: config.MeterData,
                            consumedAmountData: config.ConsumedAmountData);
                        break;
                    default:
                        throw new Exception($"Unknown consumption type: {config.Kind}");
                }
            }
        }

        private static void RecordReservedAndReleaseCapacity(
            IAudit auditClient,
            int numberOfEvents,
            int numberOfThreads,
            string eventHubsTopic,
            string eventHubsHost,
            AppConfigMeterData meterData,
            AppConfigReservedAndReleaseCapacityData data)
        {
            var reservedCapacityDestination = new AuditClientReservedCapacityMetricsDestination(auditClient);

            var groupId = Guid.NewGuid();
            var subscriptionId = meterData.SubscriptionId;
            var resourceId = meterData.ResourceId;
            var meter = meterData.Meter;

            Log.Information(
                "Sending {NumberOfEvents} {EventKind} events ({NumberOfThreads} threads) in GroupId {GroupId} to the {EventHubsTopic} topic on EventHubs {EventHubsHost}",
                numberOfEvents,
                ConsumptionKind.ReserveAndReleaseCapacity,
                numberOfThreads,
                groupId,
                eventHubsTopic,
                eventHubsHost);

            var reservedCapacity = new ReservedCapacityMetrics(reservedCapacityDestination);

            using (Operation.Time(
                "Sending {NumberOfEvents} {EventKind} events ({NumberOfThreads} threads) in GroupId {GroupId} to the {EventHubsTopic} topic on EventHubs {EventHubsHost}",
                numberOfEvents,
                ConsumptionKind.ReserveAndReleaseCapacity,
                numberOfThreads,
                groupId,
                eventHubsTopic,
                eventHubsHost))
            {
                Enumerable
                    .Range(0, numberOfEvents)
                    .AsParallel()
                    .WithDegreeOfParallelism(degreeOfParallelism: numberOfThreads)
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .Select(eventNumber =>
                    {
                        var contextReservedCapacity = reservedCapacity
                            .ForInternalContext("GroupId", $"{groupId}")
                            .ForInternalContext("EventNumber", $"{eventNumber}");

                        contextReservedCapacity
                            .Increase(
                                subscriptionId: subscriptionId,
                                resourceId: resourceId,
                                meter: meter,
                                amount: data.CapacityAmount,
                                dateTime: data.ReservedDateTime);

                        contextReservedCapacity
                            .Decrease(
                                subscriptionId: subscriptionId,
                                resourceId: resourceId,
                                meter: meter,
                                amount: data.CapacityAmount,
                                dateTime: data.ReservedDateTime);

                        return eventNumber;
                    })
                    .ToArray();
            }
        }

        private static void RecordConsumedAmount(
            IAudit auditClient,
            int numberOfEvents,
            int numberOfThreads,
            string eventHubsTopic,
            string eventHubsHost,
            AppConfigMeterData meterData,
            AppConfigConsumedAmountData consumedAmountData)
        {
            var auditConsumptionDestination = new AuditClientConsumptionMetricsDestination(auditClient);

            var groupId = Guid.NewGuid();
            var subscriptionId = meterData.SubscriptionId;
            var resourceId = meterData.ResourceId;
            var meter = meterData.Meter;
            var resourceType = consumedAmountData.ResourceType;
            var resourceName = consumedAmountData.ResourceName;

            Log.Information(
                "Sending {NumberOfEvents} {EventKind} events ({NumberOfThreads} threads) in GroupId {GroupId} to the {EventHubsTopic} topic on EventHubs {EventHubsHost}",
                numberOfEvents,
                ConsumptionKind.ConsumedAmount,
                numberOfThreads,
                groupId,
                eventHubsTopic,
                eventHubsHost);

            var consumptionClient = new ConsumptionMetrics(auditConsumptionDestination);

            using (Operation.Time(
                "Sending {NumberOfEvents} {EventKind} events ({NumberOfThreads} threads) in GroupId {GroupId} to the {EventHubsTopic} topic on EventHubs {EventHubsHost}",
                numberOfEvents,
                ConsumptionKind.ConsumedAmount,
                numberOfThreads,
                groupId,
                eventHubsTopic,
                eventHubsHost))
            {
                Enumerable
                    .Range(0, numberOfEvents)
                    .AsParallel()
                    .WithDegreeOfParallelism(degreeOfParallelism: numberOfThreads)
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .Select(eventNumber =>
                    {
                        consumptionClient
                            .ForInternalContext("GroupId", $"{groupId}")
                            .ForInternalContext("EventNumber", $"{eventNumber}")
                            .ForSubscriptionOwnerContext("Resource Type", resourceType)
                            .ForSubscriptionOwnerContext("Resource Name", resourceName)
                            .Record(
                                subscriptionId: subscriptionId,
                                resourceId: resourceId,
                                meter: meter,
                                amount: consumedAmountData.Amount,
                                reason: consumedAmountData.Reason);

                        return eventNumber;
                    })
                    .ToArray();
            }
        }
    }
}