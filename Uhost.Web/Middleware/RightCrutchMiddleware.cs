using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Services.User;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using static Uhost.Web.Common.RightRequirement;

namespace Uhost.Web.Middleware
{
    /// <summary>
    /// Костыль для работы прав
    /// </summary>
    public sealed class RightCrutchMiddleware : IDisposable
    {
        private static readonly List<RouteInfo> _targets;

        static RightCrutchMiddleware()
        {
            _targets = Common.Tools.Routes
                .Where(e => e.ControllerMethod.CustomAttributes.Any(a => a.AttributeType.IsAssignableTo<HasRightAuthorizeAttribute>()))
                .ToList();
        }

        private readonly IUserService _userService;
        private readonly IServiceScope _scope;
        private readonly RequestDelegate _next;

        public RightCrutchMiddleware(RequestDelegate next, IServiceProvider provider)
        {
            _next = next;
            _scope = provider.CreateScope();
            _userService = _scope.ServiceProvider.GetRequiredService<IUserService>();
        }

        private static async Task ForbidAsync(HttpContext context, RightRequirement requirement)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = ResponseHelper.СontentTypeJson;
            await context.Response.WriteAsync(ResponseHelper.MakeErrorData(requirement.Message).ToJson());
        }

        public async Task Invoke(HttpContext context)
        {
            var target = _targets
                .FirstOrDefault(e =>
                    e.UrlRegex.IsMatch(context.Request.Path) &&
                    context.Request.Method.Equals(e.HttpMethod, StringComparison.InvariantCultureIgnoreCase));

            if (target != null)
            {
                var requirement = _targets
                    .FirstOrDefault(e => e.UrlRegex.IsMatch(context.Request.Path) && context.Request.Method.Equals(e.HttpMethod, StringComparison.InvariantCultureIgnoreCase))?
                    .ControllerMethod?
                    .GetCustomAttribute<HasRightAuthorizeAttribute>()?
                    .ToRequirement();

                if (requirement != null && context.User.TryGetUserId(out var userId))
                {
                    var user = _userService.GetAccessData(userId);

                    switch (requirement.Rule)
                    {
                        case CombinationRule.Or:
                            if (!user.Rights.ContainsAny(requirement.Rights))
                            {
                                await ForbidAsync(context, requirement);
                                return;
                            }
                            break;
                        case CombinationRule.And:
                            if (!user.Rights.ContainsAll(requirement.Rights))

                            {
                                await ForbidAsync(context, requirement);
                                return;
                            }
                            break;
                    }
                }
            }

            await _next.Invoke(context);
        }

        public void Dispose()
        {
            _scope?.Dispose();
            _userService?.Dispose();
        }
    }
}
