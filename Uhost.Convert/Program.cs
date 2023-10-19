using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using StackExchange.Redis.Extensions.Newtonsoft;
using Uhost.Convert.Extensions;
using Uhost.Core;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Services.File;

namespace Uhost.Convert
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(CoreSettings.RedisConfig);

            services.AddDbContext<PostgreSqlDbContext>(e => e.UseNpgsql(CoreSettings.SqlConnectionString));
            services.AddDbContext<PostgreSqlLogDbContext>(e => e.UseNpgsql(CoreSettings.SqlLogConnectionString));

            services.AddScoped<IFileService, FileService>();
            //services.AddScoped<IVideoService, VideoService>();

            services.AddRabbitMqClient(CoreSettings.RabbitMqClientOptions);
            services.AddDefaultExchange();

            var provider = services.BuildServiceProvider();

            provider.RunConversionApplication();
        }
    }
}
