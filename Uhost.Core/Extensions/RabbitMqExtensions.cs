using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        private static void ThrowIfWrongQueue(string queue)
        {
            if (!_queues.Contains(queue))
            {
                var exception = new ArgumentException($"Queue \"{queue}\" is not declared in {typeof(TaskQueues).FullName}", nameof(queue));
                exception.Data["queues"] = _queues;

                throw exception;
            }
        }

        /// <summary>
        /// Registers queue
        /// </summary>
        /// <param name="service"><see cref="IQueueService"/> instance</param>
        /// <param name="queue">Queue name declared in <see cref="TaskQueues"/></param>
        public static QueueDeclareOk RegisterQueue(this IQueueService service, string queue)
        {
            ThrowIfWrongQueue(queue);

            return service.Channel.QueueDeclare(queue, durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
        }

        /// <summary>
        /// Enqueues task
        /// </summary>
        /// <typeparam name="T">Service type</typeparam>
        /// <param name="service">Queue service</param>
        /// <param name="expression">Task expression</param>
        /// <param name="queue">Queue name declared in <see cref="TaskQueues"/></param>
        public static void Enqueue<T>(this IQueueService service, Expression<Action<T>> expression, string queue)
        {
            ThrowIfWrongQueue(queue);

            var task = SerializedTask.Create(expression);

            service.Channel.BasicPublish(string.Empty, queue, null, Encoding.UTF8.GetBytes(task.ToJson()));
        }
    }
}
