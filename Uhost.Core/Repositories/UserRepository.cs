using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.User;
using QueryModel = Uhost.Core.Models.User.UserQueryModel;

namespace Uhost.Core.Repositories
{
    public class UserRepository : BaseRepository<Entity>
    {
        protected override Func<IQueryable<Entity>, IQueryable<Entity>> DbSetUpdateTransformations => e => e
            .Include(e => e.UserRoles);

        public UserRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet
                .Include(e => e.UserRoles);

            if (query.Id > 0)
            {
                q = q.Where(e => e.Id == query.Id);
            }
            if (query.Ids != null)
            {
                q = q.Where(e => query.Ids.Contains(e.Id));
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }
            if (query.ExcludedId > 0)
            {
                q = q.Where(e => e.Id != query.ExcludedId);
            }
            if (!string.IsNullOrEmpty(query.LoginOrEmail))
            {
                q = q.Where(e => e.Login == query.LoginOrEmail || e.Email == query.LoginOrEmail);
            }

            q = q.OrderBy(query);

            return q;
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query) where TModel : BaseModel<Entity>, new()
        {
            query ??= new QueryModel();
            var q = PrepareQuery(query);

            return Get<TModel>(q);
        }

        internal int UpdateLastVisitAt(int id)
        {
            return Perform(e => e.LastVisitAt = DateTime.Now, e => e.Id == id);
        }
    }
}
