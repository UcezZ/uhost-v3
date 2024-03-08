namespace Uhost.Core.Services.Scheduler
{
    public interface ISchedulerService
    {
        void ScheduleVideoConvert(int conversionStateId);
        void ScheduleVideoStreamFetch(int videoId, string url);
    }
}
