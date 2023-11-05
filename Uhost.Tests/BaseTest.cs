using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection;
using System;
using System.Text;
using Uhost.Core;
using Uhost.Core.Extensions;
using Uhost.Web.Middleware;
using Uhost.Web.Providers;
using Uhost.Web.Services.Auth;

namespace Uhost.Tests
{
    public abstract class BaseTest
    {
        protected IServiceProvider Provider { get; }

        protected BaseTest()
        {
            var services = new ServiceCollection();

            services.AddUhostCoreServices();

            services.AddScoped<IAuthService, AuthService>();

            services.AddHttpContextAccessor();

            // Кодировка
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // Проверка прав
            services.AddScoped<IAuthorizationHandler, RightAuthHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, HasRightPolicyProvider>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResultTransformer>();

            // RMQ
            services.AddRabbitMqClient(CoreSettings.RabbitMqClientOptions);

            Provider = services.BuildServiceProvider();
        }
    }
}
