using System;
using System.Threading.Tasks;
using Kmd.Logic.Audit.Client.SerilogAzureEventHubs;
using Kmd.Logic.Consumption.Client;
using Kmd.Logic.Consumption.Client.AuditClient;

namespace Kmd.Logic.Consumption.Client.Sample
{
    public class Program
    {
        static void Main(string[] args)
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

            Parallel.For(0, 1, t =>
            {
                string messageId = Guid.NewGuid().ToString();
                Console.WriteLine(string.Format("Sms sent to provider {0}", messageId));

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
