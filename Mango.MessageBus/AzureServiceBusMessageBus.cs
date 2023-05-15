
using Azure.Messaging.ServiceBus;

using Newtonsoft.Json;

using System.Text;

namespace Mango.MessageBus;

public class AzureServiceBusMessageBus : IMessageBus
{
    // can be improved
    private string connectionString = "Endpoint=sb://mangorestaurant84.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UGQakQcLwVOIc/Yo19/XTNUbUNtxpSWos+ASbMWBl98=";

    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        await using var client = new ServiceBusClient(connectionString, clientOptions);

        ServiceBusSender sender = client.CreateSender(topicName);

        var JsonMessage = JsonConvert.SerializeObject(message);

        ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonMessage))
        {
            CorrelationId = Guid.NewGuid().ToString()
        };

        await sender.SendMessagesAsync(new List<ServiceBusMessage> { finalMessage });

        await sender.DisposeAsync();
    }
}
