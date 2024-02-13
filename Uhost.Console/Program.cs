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
using Uhost.Core.Extensions;
using Uhost.Core.Services.Log;
using static System.Console;
using static Uhost.Core.Data.Entities.Log;

namespace Uhost.Console
{
    static class Program
    {
        static void Main(string[] args)
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
                .Where(e => typeof(BaseCommand).IsAssignableFrom(e) && e.IsClass)
                .ToArray();

            try
            {
                Parser.Default
                    .ParseArguments(args, commands)
                    .WithParsed<BaseCommand>(c => c.UseServiceProvider(provider).ValidateAndRun());
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);

                using (var svc = provider.GetRequiredService<ILogService>())
                {
                    svc.Add(Events.ConsoleCommandError, new { Args = args, Exception = e?.ToDetailedDataObject() }, true);
                }
            }
        }
    }
}
