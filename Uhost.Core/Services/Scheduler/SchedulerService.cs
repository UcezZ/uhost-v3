using RabbitMQ.Client.Core.DependencyInjection.Services;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Video;
using static Uhost.Core.Data.Entities.File;

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

        public void ScheduleVideoConvert(int videoId, Types type)
        {
            var typeId = (int)type;
            _queue.Enqueue<IVideoService>(e => e.Convert(videoId, typeId), TaskQueues.Conversion);
        }
    }
}
