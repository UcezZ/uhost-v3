using Uhost.Core.Data;

namespace Uhost.Core.Services.HangfireExecutor
{
    public sealed class HangfireExecutorService : BaseService, IHangfireExecutorService
    {
        public HangfireExecutorService(PostgreSqlDbContext dbContext) : base(dbContext)
        {
        }
    }
}
