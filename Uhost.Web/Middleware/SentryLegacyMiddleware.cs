using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Sentry;
using System.Threading.Tasks;

namespace Uhost.Web.Middleware
{
    public class SentryLegacyMiddleware
    {
        private readonly RequestDelegate _next;

        public SentryLegacyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next.Invoke(context);
            var ctx = context.Features.Get<IExceptionHandlerFeature>();

            if (ctx?.Error != null)
            {
                SentrySdk.CaptureException(ctx?.Error);
            }
        }
    }
}
