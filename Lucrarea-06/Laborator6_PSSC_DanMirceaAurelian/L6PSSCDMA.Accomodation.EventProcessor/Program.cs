using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using Laborator6_PSSC_DanMirceaAurelian.Events.ServiceBus;
using Laborator6_PSSC_DanMirceaAurelian.Events;

namespace Laborator6_PSSC_DanMirceaAurelian.Accomodation.EventProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAzureClients(builder =>
                {
                    builder.AddServiceBusClient("Endpoint=sb://laborator6-pssc-danmirceaaurelian.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AWVsaiKO194FlEbfmyMCniUPUl8qpmmRaz4UOJOjL44=");
                });

                services.AddSingleton<IEventListener, ServiceBusTopicEventListener>();
                services.AddSingleton<IEventHandler, ShoppingCartsPaidEventHandler>();

                services.AddHostedService<Worker>();
            });
    }
}
