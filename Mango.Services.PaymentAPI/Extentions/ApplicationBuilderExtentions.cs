using Mango.Services.PaymentAPI.Messaging;

namespace Mango.Services.PaymentAPI.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IAzureServiceBusConsumerPayment ServiceBusConsumer { get; set; }
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumerPayment>();
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
