using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog.Config;
using NLog.Targets.GraylogHttp;
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
            var graylog = new GraylogHttpTarget
            {
                Host = "localhost",
                GraylogServer = "localhost",
                GraylogPort = "12201"
            };
            var loggingConfig = new LoggingConfiguration();
            loggingConfig.AddTarget("graylog", graylog);

            var builder = Host
                .CreateDefaultBuilder(Environment.GetCommandLineArgs())
                //.ConfigureWebHost(e => e.UseStartup<Startup>())
                .ConfigureWebHostDefaults(e => e.UseStartup<Startup>())
                /*.ConfigureLogging(logging =>
                {
                    logging.AddNLog(loggingConfig);
                    logging.AddNLogWeb(loggingConfig);
                    //logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog()*/;

            _app = builder.Build();
        }

        static void Main(string[] args)
        {
            using (SentrySdk.Init(CoreSettings.SentryConfig.Configure))
            {
                _app.Run();
            }
        }
    }
}
