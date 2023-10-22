using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Uhost.Core.Data;
using Uhost.Core.Services;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Graylog;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.RestClient;
using Uhost.Core.Services.Role;
using Uhost.Core.Services.Scheduler;
using Uhost.Core.Services.User;
using Uhost.Core.Services.Video;

namespace Uhost.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
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

            services.AddSingleton<ISchedulerService, SchedulerService>();

            return services;
        }

        public static IServiceCollection AddLogWriter(this IServiceCollection services, LogWriter.Severity minSeverity = LogWriter.Severity.Info)
        {
            services.AddSingleton(new LogWriter(System.Console.Out, minSeverity));

            return services;
        }
    }
}
