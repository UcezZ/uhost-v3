using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Newtonsoft;
using System;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Services;
using Uhost.Core.Services.Email;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Graylog;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.Razor;
using Uhost.Core.Services.Register;
using Uhost.Core.Services.RestClient;
using Uhost.Core.Services.Role;
using Uhost.Core.Services.Scheduler;
using Uhost.Core.Services.User;
using Uhost.Core.Services.Video;

namespace Uhost.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Добавляет в DI основные сервисы ядра проекта
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUhostCoreServices(this IServiceCollection services)
        {
            services.AddDbContext<PostgreSqlDbContext>(e => e.UseNpgsql(CoreSettings.SqlConnectionString));
            services.AddDbContext<PostgreSqlLogDbContext>(e => e.UseNpgsql(CoreSettings.SqlLogConnectionString));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRestClientService, RestClientService>();
            services.AddScoped<IGraylogService, GraylogService>();
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRegisterService, RegisterService>();

            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddSingleton<IRazorService, RazorService>();

            // Redis
            ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(CoreSettings.RedisConfig);

            return services;
        }

        /// <summary>
        /// Добавляет <see cref="LogWriter"/> в DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="minSeverity"></param>
        /// <returns></returns>
        public static IServiceCollection AddLogWriter(this IServiceCollection services, LogWriter.Severity minSeverity = LogWriter.Severity.Info)
        {
            services.AddSingleton(new LogWriter(Console.Out, minSeverity));

            return services;
        }

        /// <summary>
        /// Создаёт экземпляр сервиса, использующего DI
        /// </summary>
        /// <typeparam name="TService">Тип сервиса</typeparam>
        /// <param name="provider">Провайдер сервисов</param>
        /// <returns></returns>
        public static TService Instantiate<TService>(this IServiceProvider provider) where TService : class
        {
            var type = typeof(TService);
            var constructor = type.GetConstructors().FirstOrDefault(e => e.GetParameters().Any());

            if (constructor == null)
            {
                throw new InvalidOperationException($"Class \"{type.FullName}\" doesn't contain parameterless constructor");
            }

            var argTypes = constructor.GetParameters();
            var args = argTypes.Select(e => provider.GetRequiredService(e.ParameterType)).ToArray();

            return constructor.Invoke(args) as TService;
        }
    }
}
