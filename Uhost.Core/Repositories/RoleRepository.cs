using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Role;
using QueryModel = Uhost.Core.Models.Role.RoleQueryModel;

namespace Uhost.Core.Repositories
{
    public class RoleRepository : BaseRepository<Entity>
    {
        protected override Func<IQueryable<Entity>, IQueryable<Entity>> DbSetUpdateTransformations => dbSet => dbSet
            .Include(e => e.RoleRights);

        public RoleRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet
                .AsNoTracking()
                .Include(e => e.UserRoles)
                .Include(e => e.RoleRights)
                .Include(e => e.Rights);

            if (query.Id > 0)
            {
                q = q.Where(e => e.Id == query.Id);
            }
            if (query.Ids != null)
            {
                q = q.Where(e => query.Ids.Contains(e.Id));
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.UserRoles.Any(r => r.UserId == query.UserId));
            }
            if (query.ExcludedId > 0)
            {
                q = q.Where(e => e.Id != query.ExcludedId);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                q = q.Where(e => e.Name == query.Name);
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }

            q = q.OrderBy(query);

            return q;
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null)
            where TModel : IEntityLoadable<Entity>, new()
        {
            query ??= new QueryModel();
            var q = PrepareQuery(query);

            return Get<TModel>(q);
        }
    }
}
