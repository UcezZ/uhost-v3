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

        public void ScheduleVideoConvert(int conversionStateId)
        {
            _client.Enqueue<IVideoService>(e => e.Convert(conversionStateId), TaskQueues.Conversion);
        }

        public void ScheduleVideoStreamFetch(int conversionStateId, string url)
        {
            _client.Enqueue<IVideoService>(e => e.FetchStream(conversionStateId, url), TaskQueues.Fetch);
        }
    }
}
