namespace Uhost.Core.Services.Scheduler
{
    public interface ISchedulerService
    {
        void ScheduleVideoConvert(int processingStateId);
        void ScheduleVideoStreamFetch(int processingStateId, string url);
    }
}
