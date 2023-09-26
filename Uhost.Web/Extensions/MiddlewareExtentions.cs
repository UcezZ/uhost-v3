using Microsoft.AspNetCore.Builder;
using Uhost.Core.Middleware;
using Uhost.Web.Attributes;

namespace Uhost.Core.Extensions
{
    public static class MiddlewareExtentions
    {
        /// <summary>
        /// Подключает <see cref="ThrottleHandler"/>.
        /// </summary>
        /// <remarks>
        /// Установить <see cref="ThrottleAttribute"/> у целевых методов контроллеров для ограничения.
        /// </remarks>
        public static IApplicationBuilder UseQueryThrottling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThrottleHandler>();
        }
    }
}
