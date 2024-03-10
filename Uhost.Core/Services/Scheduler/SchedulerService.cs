using Hangfire;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Video;

namespace Uhost.Core.Services.Scheduler
{
    public sealed class SchedulerService : ISchedulerService
    {
        private readonly BackgroundJobClient _client;

        public SchedulerService(JobStorage jobStorage)
        {
            _client = new BackgroundJobClient(jobStorage);
        }

        public void ScheduleVideoConvert(int processingStateId)
        {
            _client.Enqueue<IVideoService>(e => e.Convert(processingStateId), TaskQueues.Conversion);
        }

        public void ScheduleVideoStreamFetch(int processingStateId, string url)
        {
            _client.Enqueue<IVideoService>(e => e.FetchStream(processingStateId, url), TaskQueues.Fetch);
        }
    }
}
