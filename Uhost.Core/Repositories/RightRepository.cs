using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Right;
using QueryModel = Uhost.Core.Models.Right.RightQueryModel;

namespace Uhost.Core.Repositories
{
    public class RightRepository : BaseRepository<Entity>
    {
        public RightRepository(DbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query = null)
        {
            query ??= new QueryModel();

            IQueryable<Entity> q = DbSet;

            if (query.RoleIds != null)
            {
                q = q.Include(e => e.RoleRights)
                    .Where(e => e.RoleRights.Any(r => query.RoleIds.Contains(r.RoleId)));
            }

            return q.OrderBy(e => e.Id);
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null)
            where TModel : IEntityLoadable<Entity>, new()
        {
            query ??= new QueryModel();
            var q = PrepareQuery(query);

            return Get<TModel>(q);
        }

        internal bool CheckIds(IEnumerable<int> ids, out int invalid)
        {
            var existing = DbSet
                .Where(e => ids.Contains(e.Id))
                .Select(e => e.Id)
                .ToList();

            invalid = ids.FirstOrDefault(e => !existing.Contains(e));

            return invalid == 0;
        }
    }
}
