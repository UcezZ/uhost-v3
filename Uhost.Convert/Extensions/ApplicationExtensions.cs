using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Extensions;

namespace Uhost.Convert.Extensions
{
    static class ApplicationExtensions
    {
        private static bool _cancel;

        public static void RunConversionApplication(this IServiceProvider provider)
        {
            /*var factory = new ConnectionFactory
            {
                HostName = CoreSettings.RabbitMqClientOptions.HostName,
                Port = CoreSettings.RabbitMqClientOptions.Port,
                UserName = CoreSettings.RabbitMqClientOptions.UserName,
                Password = CoreSettings.RabbitMqClientOptions.Password
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: TaskQueues.Conversion,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
            };
            channel.BasicConsume(queue: TaskQueues.Conversion,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();*/

            var queueService = provider.GetRequiredService<IQueueService>();

            var q = queueService.RegisterQueue(TaskQueues.Conversion);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new AsyncEventingBasicConsumer(queueService.Channel);

            consumer.Received += OnReceivedAsync;

            queueService.Channel.BasicConsume(
                queue: q.QueueName,
                autoAck: true,
                consumer: consumer);

            Console.CancelKeyPress += OnCancelKeyPress;

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
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await Console.Out.WriteLineAsync($" [x] Received {message}");
            Thread.Sleep(10000);
        }
    }
}
