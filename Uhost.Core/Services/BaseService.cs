using System;
using System.Threading.Tasks;
using Uhost.Core.Data;

namespace Uhost.Core.Services
{
    public abstract class BaseService : IDisposable, IAsyncDisposable
    {
        protected readonly PostgreSqlDbContext _dbContext;

        public BaseService(PostgreSqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual void Dispose()
        {
            _dbContext.Dispose();
        }

        public virtual async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
