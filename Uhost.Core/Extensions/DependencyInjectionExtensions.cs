using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Uhost.Core.Data;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Graylog;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.RestClient;
using Uhost.Core.Services.Role;
using Uhost.Core.Services.Task.Executor;
using Uhost.Core.Services.Task.Scheduler;
using Uhost.Core.Services.User;

namespace Uhost.Core.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddUhostCoreServices(this IServiceCollection services)
        {
            services.AddDbContext<PostgreSqlDbContext>(e => e.UseNpgsql(CoreSettings.SqlConnectionString));
            services.AddDbContext<PostgreSqlLogDbContext>(e => e.UseNpgsql(CoreSettings.SqlLogConnectionString));

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRestClientService, RestClientService>();
            services.AddScoped<IGraylogService, GraylogService>();
            services.AddScoped<IExecutorService, ExecutorService>();

            services.AddSingleton<ISchedulerService, SchedulerService>();
        }
    }
}
