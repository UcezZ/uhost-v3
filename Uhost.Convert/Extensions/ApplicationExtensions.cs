using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;
using Sentry;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Convert.Extensions
{
    static class ApplicationExtensions
    {
        private static bool _cancel;
        private static IServiceProvider _provider;

        public static void RunConversionApplication(this IServiceProvider provider)
        {
            _provider ??= provider;

            var queueService = provider.GetRequiredService<IQueueService>();

            var q = queueService.RegisterQueue(TaskQueues.Conversion);

            WriteLine("[Uhost.Convert]");
            WriteLine($"Work threads: {ConvertSettings.ConverterWorkThreads}");

            CancelKeyPress += OnCancelKeyPress;

            for (var i = 0; i < ConvertSettings.ConverterWorkThreads; i++)
            {
                var consumer = new AsyncEventingBasicConsumer(queueService.Channel);

                consumer.Received += OnReceivedAsync;

                queueService.Channel.BasicConsume(
                    queue: q.QueueName,
                    autoAck: true,
                    consumer: consumer);
            }

            while (!_cancel)
            {
                Thread.Sleep(1000);
            }
        }

        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _cancel = true;
            e.Cancel = true;
        }

        private static async Task OnReceivedAsync(object model, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.Span);
            await Out.WriteLineAsync($" [x] Received {message}");

            if (SerializableTask.TryParseJson(message, out var task))
            {
                try
                {
                    task.Invoke(_provider);
                }
                catch (Exception exception)
                {
                    SentrySdk.CaptureException(exception);

                    var inner = exception.GetMostInnerException();

                    await Error.WriteLineAsync($"{inner?.Message}\r\n{inner?.StackTrace}");
                }
            }
        }
    }
}
