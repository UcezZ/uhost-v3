using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using System.Text;
using Uhost.Core.Common;
using Uhost.Core.Extensions;

namespace Uhost.Core.Services.Scheduler
{
    public sealed class SchedulerService : ISchedulerService
    {
        private readonly IQueueService _queue;

        public SchedulerService(IQueueService queue)
        {
            _queue = queue;
            _queue.RegisterQueue(TaskQueues.Conversion);
        }

        public void ScheduleTest()
        {
            _queue.Channel.BasicPublish(string.Empty, TaskQueues.Conversion, null, Encoding.UTF8.GetBytes("Жопа!"));
        }
    }
}
