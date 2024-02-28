using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Uhost.Core.Data
{
    public abstract class BaseDbContext : DbContext
    {
        protected const string _sqlNow = "now()";
        protected const string _sqlEmptyJsonArray = "[]";
        protected const string _sqlEmptyJson = "{}";

        public void DetachAllEntities()
        {
            ChangeTracker.Entries()
                .ToList()
                .ForEach(x => x.State = EntityState.Detached);
        }
    }
}
