using Microsoft.EntityFrameworkCore;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.VideoReaction;
using QueryModel = Uhost.Core.Models.VideoReaction.VideoReactionQueryModel;

namespace Uhost.Core.Repositories
{
    public class VideoReactionRepository : BaseRepository<Entity>
    {
        public VideoReactionRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

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
            if (query.VideoId > 0)
            {
                q = q.Where(e => e.VideoId == query.VideoId);
            }
            if (query.VideoIds != null)
            {
                q = q.Where(e => query.VideoIds.Contains(e.VideoId));
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }

            q = q.OrderBy(query);

            return q;
        }

        public TCollectionModel GetAllGrouped<TCollectionModel>(QueryModel query = null) where TCollectionModel : BaseCollectionModel<Entity>, new()
        {
            query ??= new QueryModel();

            return GetCollection<TCollectionModel>(PrepareQuery(query));
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null) where TModel : BaseModel<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }
    }
}
