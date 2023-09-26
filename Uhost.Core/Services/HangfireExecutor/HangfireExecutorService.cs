using Uhost.Core.Data;

namespace Uhost.Core.Services.HangfireExecutor
{
    public class HangfireExecutorService : BaseService, IHangfireExecutorService
    {
        public HangfireExecutorService(PostgreSqlDbContext dbContext) : base(dbContext)
        {
        }
    }
}
