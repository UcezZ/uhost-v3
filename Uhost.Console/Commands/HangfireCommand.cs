using CommandLine;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Uhost.Console.Properties;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Console.Commands
{
    [Verb("hangfire", HelpText = "Запуск обработки запланированных задач")]
    public sealed class HangfireCommand : BaseCommand
    {
        private static readonly IEnumerable<string> _queues;
        private bool _break;

        static HangfireCommand()
        {
            _queues = typeof(TaskQueues)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(e => e.GetValue(null))
                .OfType<string>()
                .ToList();
        }

        [Option("queue", Required = true, HelpText = "Наименования очередей задач [--queue " + TaskQueues.Default + "," + TaskQueues.Distribution + "]", Separator = ',')]
        public IEnumerable<string> Queues { get; set; }

        [Option("threads", Required = false, HelpText = "Количество одновременно выполняемых задач. По умолчанию кол-во потоков ЦП * 2")]
        public int Threads { get; set; } = Environment.ProcessorCount * 2;

        protected override bool Validate()
        {
            var qFailed = Queues.Except(_queues);

            if (qFailed.Any())
            {
                AddValidationError(ConsoleStrings.Hangfire_Error_QueueFail.Format(qFailed.Join(", "), _queues.Join(", ")));
            }
            if (Threads < 1)
            {
                Threads = 1;
            }

            return base.Validate();
        }

        protected override void Run()
        {
            CancelKeyPress += OnCtrlBreakCaptured;

            var jobActivator = Provider.GetRequiredService<JobActivator>();
            var jobStorage = Provider.GetRequiredService<JobStorage>();

            // не выполняем провалившиеся задания
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            var options = new BackgroundJobServerOptions
            {
                Queues = Queues.ToArray(),
                WorkerCount = Threads,
                Activator = jobActivator
            };

            using (var hangfireServer = new BackgroundJobServer(options, jobStorage))
            {
                while (!_break)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void OnCtrlBreakCaptured(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _break = true;
        }
    }
}
