﻿using System;
using System.Threading.Tasks;
using Kmd.Logic.Audit.Client.SerilogAzureEventHubs;
using Kmd.Logic.Consumption.Client;
using Kmd.Logic.Consumption.Client.AuditClient;

namespace Kmd.Logic.Consumption.Client.Sample
{
    public static class Program
    {
        public static void Main()
        {
            var clientConfig = new SerilogAzureEventHubsAuditClientConfiguration
            {
                ConnectionString = @"Endpoint=sb://kmdais-consumptiondev-eventhub.servicebus.windows.net/;SharedAccessKeyName=primaryAuditIngestKey;SharedAccessKey=BTz9MTaQs9dofKGGUyADIMKvhmyBSuIcsK748MiYIVA=;EntityPath=audit",
                EnrichFromLogContext = true
            };

            var serilogSeqAuditClient = new SerilogAzureEventHubsAuditClient(clientConfig);
            IConsumptionMetricsDestination consumptionDestination = new AuditClientConsumptionMetricsDestination(serilogSeqAuditClient);
            IConsumptionMetrics consumption = new ConsumptionClient(consumptionDestination);
            var subscriptionId = Guid.NewGuid();
            var resourceId = Guid.NewGuid();
            var consumptionType = "Sent to BYO Provider";
            var resourceType = "SMS";
            var resourceName = "FRIE PROD";

            Parallel.For(0, 10, t =>
            {
                string messageId = Guid.NewGuid().ToString();
                Console.WriteLine($"Sms sent to provider {messageId}");

                consumption.ForContext("ResourceType", resourceType)
                        .ForContext("ResourceName", resourceName)
                        .Record(
                            subscriptionId: subscriptionId,
                            resourceId: resourceId,
                            consumptionType: consumptionType,
                            consumptionAmount: +10);
            });
        }
    }
}
