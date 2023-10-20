using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using Uhost.Convert.Extensions;
using Uhost.Core;
using Uhost.Core.Extensions;

namespace Uhost.Convert
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddUhostCoreServices();

            services.AddRabbitMqClient(CoreSettings.RabbitMqClientOptions);
            services.AddDefaultExchange();

            var provider = services.BuildServiceProvider();

            provider.RunConversionApplication();
        }
    }
}
