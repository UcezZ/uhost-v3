using Hangfire;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Video;
using static Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Services.Scheduler
{
    public sealed class SchedulerService : ISchedulerService
    {
        private readonly BackgroundJobClient _client;

        public SchedulerService(JobStorage jobStorage)
        {
            _client = new BackgroundJobClient(jobStorage);
        }

        public void ScheduleVideoConvert(int videoId, Types type)
        {
            _client.Enqueue<IVideoService>(e => e.Convert(videoId, type), TaskQueues.Conversion);
        }

        public void ScheduleVideoStreamConvert(int videoId, string url)
        {
            _client.Enqueue<IVideoService>(e => e.FetchUrl(videoId, url), TaskQueues.Conversion);
        }
    }
}
