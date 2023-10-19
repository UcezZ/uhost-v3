using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Common;

namespace Uhost.Core.Extensions
{
    public static class RabbitMqExtensions
    {
        public const string DefaultExchange = "uhost";

        private static readonly IEnumerable<string> _queues;

        static RabbitMqExtensions()
        {
            _queues = typeof(TaskQueues)
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(e => e.FieldType == typeof(string))
                .Select(e => e.GetValue(null))
                .OfType<string>()
                .ToArray();
        }

        public static void AddDefaultExchange(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddProductionExchange(DefaultExchange, new RabbitMqExchangeOptions());
        }

        /// <summary>
        /// Registers queue
        /// </summary>
        /// <param name="service"><see cref="IQueueService"/> instance</param>
        /// <param name="queue">Queue name declared in <see cref="TaskQueues"/></param>
        public static QueueDeclareOk RegisterQueue(this IQueueService service, string queue)
        {
            if (!_queues.Contains(queue))
            {
                throw new ArgumentException($"Queue name must be declared in {typeof(TaskQueues).FullName}");
            }

            return service.Channel.QueueDeclare(queue, durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
        }
    }
}
