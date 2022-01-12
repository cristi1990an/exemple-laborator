using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Laborator_6_PSSC.Events.ServiceBus;
using Laborator_6_PSSC.Events;

namespace Laborator_6_PSSC.Accomodation.EventProcessor
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
                    builder.AddServiceBusClient("Endpoint=sb://pssccristi.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=OQIJOlYLTTud7NCFoLPA45+K0GkVgmSADTcNH57RdE0=");
                });

                services.AddSingleton<IEventListener, ServiceBusTopicEventListener>();
                services.AddSingleton<IEventHandler, ShoppingCartsPaidEventHandler>();

                services.AddHostedService<Worker>();
            });
    }
}
