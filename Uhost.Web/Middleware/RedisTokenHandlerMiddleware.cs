using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Sentry;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Web.Common;
using Uhost.Web.Properties;
using static System.Console;

namespace Uhost.Web.Middleware
{
    /// <summary>
    /// Костыль для асинхронной валидации JWT, поскольку MS это не предусмотрели :)
    /// </summary>
    public class RedisTokenHandlerMiddleware
    {
        private const string _keyPrefix = "authtoken";
        public static string RedisKey<TId, TJti>(TId id, TJti jti) =>
            $"{_keyPrefix}_{id}_{jti}";

        private static readonly List<RouteInfo> _targets;

        static RedisTokenHandlerMiddleware()
        {
            // маршруты
            _targets = Common.Tools.Routes

                // у методов контроллеров или контроллеров которых есть атрибут
                .Where(e =>
                    e.ControllerMethod.CustomAttributes.Any(a => a.AttributeType == typeof(AuthorizeAttribute)) ||
                    e.ControllerMethod.DeclaringType?.CustomAttributes != null &&
                    e.ControllerMethod.DeclaringType.CustomAttributes.Any(a => a.AttributeType == typeof(AuthorizeAttribute)))

                // commit
                .ToList();
        }

        private readonly IRedisDatabase _redis;
        private readonly RequestDelegate _next;

        public RedisTokenHandlerMiddleware(IRedisClientFactory redisFactory, RequestDelegate next)
        {
            _redis = redisFactory.GetDefaultRedisDatabase();
            _next = next;
        }

        private static async Task ForbidAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            context.Response.ContentType = ResponseHelper.СontentTypeJson;
            await context.Response.WriteAsync(ResponseHelper.MakeErrorData(new[]
            {
                new { ErrorCode = HttpStatusCode.Unauthorized, ErrorStr = ApiStrings.Auth_Error_Unauthorized }
            }).ToJson());
        }

        public async Task Invoke(HttpContext context)
        {
            var target = _targets
                .FirstOrDefault(e =>
                    e.UrlRegex.IsMatch(context.Request.Path) &&
                    context.Request.Method.Equals(e.HttpMethod, StringComparison.InvariantCultureIgnoreCase));

            var timer = new Timer();

            if (target != null)
            {
                try
                {
                    if (!context.User.TryGetUserId(out var userId) || userId <= 0 || !context.User.TryGetJti(out var jti))
                    {
                        await ForbidAsync(context);
                        return;
                    }

                    var key = RedisKey(userId, jti);
                    var exists = await _redis.ExistsAsync(key);

                    if (!exists)
                    {
                        await ForbidAsync(context);
                        return;
                    }
                }
                catch (Exception e)
                {
                    if (LocalEnvironment.IsDev)
                    {
                        var inner = e.InnerException;

                        while (e?.InnerException != null)
                        {
                            inner = e.InnerException;
                        }

                        await new
                        {
                            e.Message,
                            e.StackTrace,
                            InnerMessage = inner?.Message,
                            InnerStacktrace = inner?.StackTrace
                        }.DumpAsync();
                    }
                    else
                    {
                        SentrySdk.CaptureException(e);
                    }

                    await ForbidAsync(context);
                    return;
                }
            }

            var elapsed = timer.Milliseconds;

            if (LocalEnvironment.IsDev)
            {
                if (elapsed > 50)
                {
                    ForegroundColor = ConsoleColor.Yellow;
                }

                WriteLine($"Redis token validation time elapsed: {elapsed}ms");

                ResetColor();
            }

            await _next.Invoke(context);
        }
    }
}
