using Hangfire;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Uhost.Core.Common;

namespace Uhost.Core.Extensions
{
   public static  class HangfireExtensions
    {
        private static readonly string[] _queues;

        static HangfireExtensions()
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

        public static void Enqueue(this BackgroundJobClient client,Expression<Action> expression, string queue)
        {
            ThrowIfWrongQueue(queue);
            client.Create(expression, new EnqueuedState(queue));
        }

        public static void Enqueue<TService>(this BackgroundJobClient client, Expression<Action<TService>> expression, string queue)
        {
            ThrowIfWrongQueue(queue);
            client.Create(expression, new EnqueuedState(queue));
        }
    }
}
