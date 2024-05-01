using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Uhost.Console.Attributes;

namespace Uhost.Tests
{
    internal static class HangfireExtensions
    {
        /// <summary>
        /// Запуск выполнения очередей
        /// </summary>
        /// <param name="provider">Провайдер севисов</param>
        /// <param name="timeoutMilis">Максимальная длительность выполнения</param>
        /// <param name="queues">Выполняемые очереди</param>
        public static void RunHangfire(this IServiceProvider provider, int timeoutMilis, params string[] queues)
        {
            var jobActivator = provider.GetRequiredService<JobActivator>();
            var jobStorage = provider.GetRequiredService<JobStorage>();

            // не выполняем провалившиеся задания
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            // добавляем Sentry
            GlobalJobFilters.Filters.Add(new SentryJobFilterAttribute());

            var options = new BackgroundJobServerOptions
            {
                Queues = queues,
                WorkerCount = 1,
                Activator = jobActivator
            };

            using (var hangfireServer = new BackgroundJobServer(options, jobStorage))
            {
                Thread.Sleep(timeoutMilis);
            }
        }
    }
}
