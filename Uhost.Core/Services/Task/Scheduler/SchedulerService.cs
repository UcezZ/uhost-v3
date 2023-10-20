using RabbitMQ.Client.Core.DependencyInjection.Services;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Task.Executor;

namespace Uhost.Core.Services.Task.Scheduler
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
            _queue.Enqueue<IExecutorService>(e => e.Test(), TaskQueues.Conversion);
        }
    }
}
