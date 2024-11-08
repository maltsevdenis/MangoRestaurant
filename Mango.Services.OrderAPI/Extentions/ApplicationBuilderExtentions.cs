﻿using Mango.Services.OrderAPI.Messaging;

namespace Mango.Services.OrderAPI.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IAzureServiceBusConsumerOrder ServiceBusConsumer { get; set; }
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumerOrder>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(OnStart);
        hostApplicationLife.ApplicationStopped.Register(OnStop);

        return app;
    }

    public static void OnStart()
    {
        ServiceBusConsumer.Start();
    }

    public static void OnStop()
    {
        ServiceBusConsumer.Stop();
    }
}
