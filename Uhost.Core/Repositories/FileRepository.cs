using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.File;
using QueryModel = Uhost.Core.Models.File.FileQueryModel;

namespace Uhost.Core.Repositories
{
    public class FileRepository : BaseRepository<Entity>
    {
        public FileRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet;

            if (query.Id > 0)
            {
                q = q.Where(e => e.Id == query.Id);
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.UserId == query.UserId);
            }
            if (query.DynId > 0)
            {
                q = q.Where(e => e.DynId == query.DynId);
            }
            if (query.DynIds != null)
            {
                q = q.Where(e => query.DynIds.Contains((int)e.DynId));
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                q = q.Where(e => e.Type == query.Type);
            }
            if (query.Types != null)
            {
                q = q.Where(e => query.Types.Contains(e.Type));
            }
            if (query.DynName != null)
            {
                q = q.Where(e => e.DynName == query.DynName);
            }
            if (query.Token != null)
            {
                q = q.Where(e => e.Token == query.Token);
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }

            return q.OrderBy(query);
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query) where TModel : BaseModel<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }
    }
}
