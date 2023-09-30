using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Sentry;
using System;
using Uhost.Core;

namespace Uhost.Web
{
    class Program
    {
        private static readonly IHost _app;
        internal static IServiceProvider Provider => _app.Services;

        static Program()
        {
            var builder = Host
                .CreateDefaultBuilder(Environment.GetCommandLineArgs())
                .ConfigureWebHostDefaults(e => e.UseStartup<Startup>());

            _app = builder.Build();
        }

        static void Main(string[] args)
        {
            using (SentrySdk.Init(CoreSettings.SentryConfig.Configure))
            {
                try
                {
                    _app.Run();
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                    throw;
                }
            }
        }
    }
}
