﻿using Azure.Messaging.ServiceBus;

using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;

using Newtonsoft.Json;

using PaymentProcessor;

using System.Text;

namespace Mango.Services.PaymentAPI.Messaging;

public class AzureServiceBusConsumerPayment : IAzureServiceBusConsumerPayment
{
    private readonly string serviceBusConnectionString;
    private readonly string subscriptionPayment;
    private readonly string orderPaymentProcessTopic;
    private readonly string orderUpdatePaymentResultTopic;

    private ServiceBusProcessor orderPaymentProcessor;
    private readonly IProcessPayment _processPayment;
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;

    public AzureServiceBusConsumerPayment(IProcessPayment processPayment, IConfiguration configuration, IMessageBus messageBus)
    {
        _processPayment = processPayment;
        _configuration = configuration;
        _messageBus = messageBus;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        
        subscriptionPayment = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
        orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
        orderUpdatePaymentResultTopic = _configuration.GetValue<string>("orderupdatepaymentresulttopic");

        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        var client = new ServiceBusClient(serviceBusConnectionString, clientOptions);
        orderPaymentProcessor = client.CreateProcessor(orderPaymentProcessTopic, subscriptionPayment);

    }

    public async Task Start()
    {
        orderPaymentProcessor.ProcessMessageAsync += ProcessPayments;
        orderPaymentProcessor.ProcessErrorAsync += ErrorHandler;
        await orderPaymentProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await orderPaymentProcessor.StopProcessingAsync();
        await orderPaymentProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task ProcessPayments(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body);

        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            Status = result,
            OrderId=paymentRequestMessage.OrderId,
            Email=paymentRequestMessage.Email,
        };

        try
        {
            await _messageBus.PublishMessage(updatePaymentResultMessage, orderUpdatePaymentResultTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception)
        {
            throw;
        }

    }
}
