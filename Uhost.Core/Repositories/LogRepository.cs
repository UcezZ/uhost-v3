using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.Log;
using QueryModel = Uhost.Core.Models.Log.LogQueryModel;

namespace Uhost.Core.Repositories
{
    public class LogRepository : BaseRepository<Entity>
    {
        public LogRepository(PostgreSqlLogDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet;

            if (query.EventsParsed != null)
            {
                q = q.Where(e => query.EventsParsed.Select(e => (int)e).Contains(e.EventId));
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.UserId == query.UserId);
            }

            return q.OrderBy(query);
        }

        public void Add(Entity entity)
        {
            DbSet.Add(entity);
            Save();
        }
    }
}
