﻿using Mango.Services.Email.Messaging;

namespace Mango.Services.Email.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
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
