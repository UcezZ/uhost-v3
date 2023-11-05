using CommandLine;
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
using Uhost.Core.Services;
using static System.Console;
using static Uhost.Core.Services.LogWriter;

namespace Uhost.Console.Commands
{
    [Verb("taskexecutor", HelpText = "Запуск обработки запланированных задач")]
    public sealed class TaskExecutorCommand : BaseCommand
    {
        [Option("queue", Required = true, HelpText = "Наименование очереди задач")]
        public string Queue { get; set; }

        private static bool _cancel;
        private LogWriter _logger;
        private readonly SemaphoreSlim _semaphore;

        public TaskExecutorCommand()
        {
            _semaphore = new SemaphoreSlim(ConsoleSettings.TaskExecutorWorkThreads);
        }

        public override void Run()
        {
            _logger = GetRequiredService<LogWriter>();
            var service = GetRequiredService<IQueueService>();
            var q = service.RegisterQueue(Queue);
            service.Channel.BasicQos(0, ConsoleSettings.TaskExecutorWorkThreads, false);

            CancelKeyPress += OnCancelKeyPress;

            for (var i = 0; i < ConsoleSettings.TaskExecutorWorkThreads; i++)
            {
                var consumer = new AsyncEventingBasicConsumer(service.Channel);

                consumer.Received += OnReceivedAsync;

                var tk = service.Channel.BasicConsume(
                    queue: q.QueueName,
                    autoAck: true,
                    consumerTag: $"worker-{Queue}-{i}",
                    consumer: consumer);

                _logger.WriteLine($"Started consumer with tag \"{tk}\"", Severity.Info);
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

        private async Task ProcessOneTask(string message, BasicDeliverEventArgs e)
        {
            await _semaphore.WaitAsync();
            try
            {
                await _logger.WriteLineAsync($"[{e.ConsumerTag}] started processing #{e.DeliveryTag}", Severity.Info);

                if (SerializedTask.TryParseJson(message, out var task))
                {
                    try
                    {
                        await task.InvokeAsync(Provider);
                    }
                    catch (Exception exception)
                    {
                        SentrySdk.CaptureException(exception);
                        var inner = exception.GetMostInnerException();
                        await _logger.WriteLineAsync($"Error processing task {e.DeliveryTag}: {inner?.Message}\r\n{inner?.StackTrace}", Severity.Error);
                    }

                    await _logger.WriteLineAsync($"[{e.ConsumerTag}] task #{e.DeliveryTag} completed", Severity.Info);
                }
                else
                {
                    await _logger.WriteLineAsync($"[{e.ConsumerTag}] the received message #{e.DeliveryTag} is not a task", Severity.Warn);
                }
                Thread.Sleep(3000);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task OnReceivedAsync(object model, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.Span);
            await _logger.WriteLineAsync($"[{e.ConsumerTag}] received a message #{e.DeliveryTag}", Severity.Info);
            var thread = new Thread(async () => await ProcessOneTask(message, e));
            thread.Start();
        }
    }
}
