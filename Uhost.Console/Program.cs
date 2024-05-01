using CommandLine;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Uhost.Console.Commands;
using Uhost.Console.Common;
using Uhost.Core;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Log;
using static System.Console;
using static Uhost.Core.Data.Entities.Log;

namespace Uhost.Console
{
    static class Program
    {
        static int Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddUhostCoreServices();
            services.AddLogWriter();
            services.AddSingleton<JobActivator, HangfireJobActivator>();
            var provider = services.BuildServiceProvider();

            WriteLine();
            CancelKeyPress += (s, e) => Process.GetCurrentProcess().Close();

            // определяем команды по релизации BaseCommand
            var commands = Assembly
                .GetAssembly(typeof(BaseCommand))
                .GetTypes()
                .Where(e => typeof(BaseCommand).IsAssignableFrom(e) && e.IsClass && !e.IsAbstract)
                .ToArray();

            using (SentrySdk.Init(CoreSettings.SentryConfig.Configure))
            {
                try
                {
                    Parser.Default
                        .ParseArguments(args, commands)
                        .WithParsed<BaseCommand>(c => c.UseServiceProvider(provider).ValidateAndRun());

                    return 0;
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);

                    using (var svc = provider.GetRequiredService<ILogService>())
                    {
                        svc.Add(Events.ConsoleCommandError, new { Args = args, Exception = e?.ToDetailedDataObject() }, writeToStdOut: true);
                    }

                    return 1;
                }
            }
        }
    }
}
