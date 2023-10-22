using Microsoft.EntityFrameworkCore;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Video;
using QueryModel = Uhost.Core.Models.Video.VideoQueryModel;

namespace Uhost.Core.Repositories
{
    public class VideoRepository : BaseRepository<Entity>
    {
        public VideoRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet
                .Include(e => e.User);

            if (query.Id > 0)
            {
                q = q.Where(e => e.Id == query.Id);
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.UserId == query.UserId);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                // q = q.Where(e => EF.Functions.Wo EF.Functions.ILike(e.Name, "%" + query.Name + "%"));
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

            return Get<TModel>(PrepareQuery(query));
        }
    }
}
