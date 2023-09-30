using Microsoft.AspNetCore.Http;
using Sentry;
using System;
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
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                throw;
            }
        }
    }
}
