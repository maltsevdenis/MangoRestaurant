﻿using Azure.Messaging.ServiceBus;

using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Repository;

using Newtonsoft.Json;

using System.Text;

namespace Mango.Services.OrderAPI.Messaging;

public class AzureServiceBusConsumerOrder : IAzureServiceBusConsumerOrder
{
    private readonly string serviceBusConnectionString;
    private readonly string subscriptionNameCheckout;
    private readonly string checkoutMessageTopic;
    private readonly string orderPaymentProcessTopic;
    private readonly string orderUpdatePaymentResultTopic;
    private readonly OrderRepository _orderRepository;

    private ServiceBusProcessor checkOutProcessor;
    private ServiceBusProcessor orderUpdatePaymentStatusProcessor;

    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;

    public AzureServiceBusConsumerOrder(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
    {
        _orderRepository = orderRepository;
        _configuration = configuration;
        _messageBus = messageBus;

        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        subscriptionNameCheckout = _configuration.GetValue<string>("SubscriptionCheckout");
        checkoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
        orderPaymentProcessTopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
        orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

        var clientOptions = new ServiceBusClientOptions()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        var client = new ServiceBusClient(serviceBusConnectionString, clientOptions);
        checkOutProcessor = client.CreateProcessor(checkoutMessageTopic);
        orderUpdatePaymentStatusProcessor = client.CreateProcessor(orderUpdatePaymentResultTopic, subscriptionNameCheckout);
    }

    public async Task Start()
    {
        checkOutProcessor.ProcessMessageAsync += OncheckOutMessageReceived;
        checkOutProcessor.ProcessErrorAsync += ErrorHandler;
        await checkOutProcessor.StartProcessingAsync();

        orderUpdatePaymentStatusProcessor.ProcessMessageAsync += OrderPaymentUpdateReceived;
        orderUpdatePaymentStatusProcessor.ProcessErrorAsync += ErrorHandler;
        await orderUpdatePaymentStatusProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    { 
        await checkOutProcessor.StopProcessingAsync();
        await checkOutProcessor.DisposeAsync();

        await orderUpdatePaymentStatusProcessor.StopProcessingAsync();
        await orderUpdatePaymentStatusProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }

    private async Task OncheckOutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body);

        OrderHeader orderHeader = new()
        {
            UserId = checkoutHeaderDto.UserId,
            FirstName = checkoutHeaderDto.FirstName,
            LastName = checkoutHeaderDto.LastName,
            OrderDetails = new List<OrderDetails>(),
            CardNumber = checkoutHeaderDto.CardNumber,
            CouponCode = checkoutHeaderDto.CouponCode,
            CVV = checkoutHeaderDto.CVV,
            DiscountTotal = checkoutHeaderDto.DiscountTotal,
            Email = checkoutHeaderDto.Email,
            ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            OrderTotal = checkoutHeaderDto.OrderTotal,
            PaymentStatus = false,
            Phone = checkoutHeaderDto.Phone,
            PickupDateTime = checkoutHeaderDto.PickupDateTime
        };

        foreach(var detailList in checkoutHeaderDto.CartDetails)
        {
            OrderDetails orderDetails = new()
            {
                ProductId = detailList.ProductId,
                ProductName = detailList.Product.Name,
                Price = detailList.Product.Price,
                Count = detailList.Count
            };

            orderHeader.CartTotalItems += detailList.Count;
            orderHeader.OrderDetails.Add(orderDetails);
        }

        await _orderRepository.AddOrder(orderHeader);

        PaymentRequestMessage paymentRequestMessage = new()
        {
            Name = orderHeader.FirstName + " " + orderHeader.LastName,
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            ExpiryMonthYear = orderHeader.ExpiryMonthYear,
            OrderId = orderHeader.OrderHeaderId,
            OrderTotal = orderHeader.OrderTotal
        };

        try
        {
            await _messageBus.PublishMessage(paymentRequestMessage, orderPaymentProcessTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task OrderPaymentUpdateReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        UpdatePaymentResultMessage paymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(body);

        await _orderRepository.UpdateOrderPaymentStatus(paymentResultMessage.OrderId, paymentResultMessage.Status);
        await args.CompleteMessageAsync(args.Message);
    }
}
