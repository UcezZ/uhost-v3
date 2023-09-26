using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Common;
using Entity = Uhost.Core.Data.Entities.Right;

namespace Uhost.Core.Repositories
{
    public class RightRepository : BaseRepository<Entity>
    {
        public RightRepository(DbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery() => DbSet;

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
