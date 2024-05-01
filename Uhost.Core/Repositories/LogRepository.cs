using Microsoft.EntityFrameworkCore;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Log;
using QueryModel = Uhost.Core.Models.Log.LogQueryModel;

namespace Uhost.Core.Repositories
{
    public class LogRepository : BaseRepository<Entity>
    {
        public LogRepository(PostgreSqlLogDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet
                .AsNoTracking();

            if (query.Id > 0)
            {
                q = q.Where(e => e.Id == query.Id);
            }
            if (query.IdFrom > 0)
            {
                q = q.Where(e => e.Id >= query.IdFrom);
            }
            if (query.IdTo > 0)
            {
                q = q.Where(e => e.Id <= query.IdTo);
            }
            if (query.EventsParsed != null)
            {
                q = q.Where(e => query.EventsParsed.Select(e => (int)e).Contains(e.EventId));
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.UserId == query.UserId);
            }
            if (query.DateFromParsed != null)
            {
                q = q.Where(e => e.CreatedAt >= query.DateFromParsed);
            }
            if (query.DateToParsed != null)
            {
                q = q.Where(e => e.CreatedAt <= query.DateToParsed);
            }

            return q.OrderBy(query);
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null) where TModel : IEntityLoadable<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }

        public void Add(Entity entity)
        {
            DbSet.Add(entity);
            Save();
        }
    }
}
