using static Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Services.Scheduler
{
    public interface ISchedulerService
    {
        void ScheduleVideoConvert(int videoId, Types type);
    }
}
