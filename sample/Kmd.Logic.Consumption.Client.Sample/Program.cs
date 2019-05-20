using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                Log.Fatal("Please provider the connection string by: {Args}", "--EventHubsConnectionString=\"XXX\"");
                return;
            }

            var clientConfig = new SerilogAzureEventHubsAuditClientConfiguration
            {
                ConnectionString = config.EventHubsConnectionString,
                EventSource = config.EventSource ?? $"Consumption client sample on {Environment.MachineName}",
            };
            var eventHubsTopic = clientConfig.AuditEventTopic;
            var eventHubsHost = new Microsoft.Azure.EventHubs.EventHubsConnectionStringBuilder(clientConfig.ConnectionString).Endpoint;

            var groupId = Guid.NewGuid();
            var subscriptionId = config.ConsumptionData.SubscriptionId;
            var resourceId = config.ConsumptionData.ResourceId;
            var meter = config.ConsumptionData.Meter;
            var resourceType = config.ConsumptionData.ResourceType;
            var resourceName = config.ConsumptionData.ResourceName;

            using (var auditClient = new SerilogAzureEventHubsAuditClient(clientConfig))
            {
                var auditConsumptionDestination = new AuditClientConsumptionMetricsDestination(auditClient);
                var consumptionClient = new ConsumptionClient(auditConsumptionDestination);

                Log.Information(
                    "Sending {NumberOfEvents} events ({NumberOfThreads} threads) in GroupId {GroupId} to the {EventHubsTopic} topic on EventHubs {EventHubsHost}",
                    config.NumberOfEvents,
                    config.NumberOfThreads,
                    groupId,
                    eventHubsTopic,
                    eventHubsHost);

                using (Operation.Time(
                    "Sending {NumberOfEvents} events ({NumberOfThreads} threads) in GroupId to the {EventHubsTopic} topic on EventHubs {EventHubsHost}",
                    config.NumberOfEvents,
                    config.NumberOfThreads,
                    groupId,
                    eventHubsTopic,
                    eventHubsHost))
                {
                    Enumerable
                        .Range(0, config.NumberOfEvents)
                        .AsParallel()
                        .WithDegreeOfParallelism(degreeOfParallelism: config.NumberOfThreads)
                        .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                        .Select(eventNumber =>
                        {
                            consumptionClient
                                .ForInternalContext("GroupId", $"{groupId}")
                                .ForSubscriptionOwnerContext("Resource Type", resourceType)
                                .ForSubscriptionOwnerContext("Resource Name", resourceName)
                                .ForInternalContext("EventNumber", $"{eventNumber}")
                                .Record(
                                    subscriptionId: subscriptionId,
                                    resourceId: resourceId,
                                    meter: meter,
                                    amount: config.ConsumptionData.Amount,
                                    reason: config.ConsumptionData.Reason);

                            return eventNumber;
                        })
                        .ToArray();
                }
            }
        }
    }
}