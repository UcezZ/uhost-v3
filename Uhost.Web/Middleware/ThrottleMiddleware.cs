using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Services.RedisSwitcher;
using Uhost.Web.Attributes;
using Uhost.Web.Common;
using Uhost.Web.Properties;

namespace Uhost.Web.Middleware
{
    /// <summary>
    /// Троттлинг запросов через <see cref="ThrottleAttribute"/>.
    /// </summary>
    public class ThrottleMiddleware
    {
        private const string _redisKeyPrefix = "throttle";
        private const string _lua = @"local key = @Key
local requests = tonumber(redis.call('GET', key) or '-1')
local max_requests = tonumber(@Count)
local expiry = tonumber(@SpanSeconds)

if (requests == -1) or (requests < max_requests) then
  redis.call('INCR', key)
  redis.call('EXPIRE', key, expiry)
  return 1
else
  return 0
end";
        private static readonly IEnumerable<RouteInfo> _targets;

        static ThrottleMiddleware()
        {
            // маршруты
            _targets = Common.Tools.Routes

                // у методов контроллеров которых есть атрибут
                .Where(e => e.ControllerMethod.CustomAttributes.Any(a => a.AttributeType == typeof(ThrottleAttribute)))

                // commit
                .ToList();
        }

        private readonly RequestDelegate _next;
        private readonly IRedisSwitcherService _redis;

        public ThrottleMiddleware(RequestDelegate next, IRedisSwitcherService redis)
        {
            _next = next;
            _redis = redis;
        }

        public async Task Invoke(HttpContext context)
        {
            var attribute = _targets
                .FirstOrDefault(e => e.UrlRegex.IsMatch(context.Request.Path) && context.Request.Method.Equals(e.HttpMethod, StringComparison.InvariantCultureIgnoreCase))?
                .ControllerMethod?
                .CustomAttributes?
                .FirstOrDefault(e => e.AttributeType == typeof(ThrottleAttribute))?
                .ToAttributeInstance<ThrottleAttribute>();

            if (attribute != null)
            {
                if (await Check(context, attribute))
                {
                    await _next(context);
                }
                else
                {
                    var data = ResponseHelper.MakeErrorData(ApiStrings.Common_Error_TooManyRequests);
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsJsonAsync(data);
                }

                return;
            }

            await _next(context);
        }

        /// <summary>
        /// Проверка, что не превышен лимит запросов на метод
        /// </summary>
        private async Task<bool> Check(HttpContext context, ThrottleAttribute attribute)
        {
            var redisKey = $"{_redisKeyPrefix}_{context.ResolveClientIp()}";
            var prepared = LuaScript.Prepare(_lua);

            var result = await _redis.ExecuteAsync(async e => await prepared.EvaluateAsync(e, new
            {
                Key = redisKey,
                attribute.Count,
                attribute.SpanSeconds
            }));

            return (int)result == 1;
        }
    }
}
