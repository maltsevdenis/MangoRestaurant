﻿using Azure.Messaging.ServiceBus;

using Mango.MessageBus;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Repository;

using Newtonsoft.Json;

using System.Text;

namespace Mango.Services.Email.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string subscriptionEmail;
    private readonly string orderUpdatePaymentResultTopic;
    private readonly EmailRepository _emailRepository;

    private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

    private readonly IConfiguration _configuration;

    public AzureServiceBusConsumer(EmailRepository orderRepository, IConfiguration configuration)
    {
        _emailRepository = orderRepository;
        _configuration = configuration;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        subscriptionEmail = _configuration.GetValue<string>("SubscriptionName");
        orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        var client = new ServiceBusClient(serviceBusConnectionString, clientOptions);
        orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionEmail);
    }

    public async Task Start()
    {
        orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OnOrderPaymentUpdateReceived;
        orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        await orderUpdatePaymentStatusProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OnOrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        UpdatePaymentResultMessage objMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);
        try
        {
            await _emailRepository.SendAndLogEmail(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
