﻿using Microsoft.EntityFrameworkCore;
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

            q = q.OrderBy(query);

            return q;
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query) where TModel : BaseModel<Entity>, new()
        {
            query ??= new QueryModel();
            var q = PrepareQuery(query);

            return Get<TModel>(q);
        }
    }
}
