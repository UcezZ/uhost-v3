using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

            q = q.OrderBy(query);

            return q;
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null)
            where TModel : IEntityLoadable<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }

        public Dictionary<Entity.Reactions, int> GetReactionsByOneVideo(int videoId)
        {
            var q = from e in DbSet.Where(e => e.VideoId == videoId)
                    group e by e.Value
                    into g
                    select new { g.Key, Count = g.Count() };

            var reactions = q
                .AsEnumerable()
                .Select(e => new { Value = e.Key.ParseEnum<Entity.Reactions>(), e.Count })
                .Where(e => e.Value is Entity.Reactions)
                .ToDictionary(e => (Entity.Reactions)e.Value, e => e.Count);

            return reactions;
        }

        public Dictionary<int, Dictionary<Entity.Reactions, int>> GetReactionsByVideos(IEnumerable<int> videoIds)
        {
            var q = from e in DbSet.Where(e => videoIds.Contains(e.VideoId))
                    group e by new { e.Id, e.Value }
                    into groupedByVideo
                    select new { groupedByVideo.Key, Count = groupedByVideo.Count() };

            var reactions = q
                .AsEnumerable()
                .Select(e => new { e.Key.Id, Value = e.Key.Value.ParseEnum<Entity.Reactions>(), e.Count })
                .Where(e => e.Value is Entity.Reactions)
                .Select(e => new { e.Id, Value = (Entity.Reactions)e.Value, e.Count })
                .ToList();

            return reactions
                .GroupBy(e => e.Id)
                .ToDictionary(e => e.Key, e => e.ToDictionary(r => r.Value, r => r.Count));
        }

        public Entity.Reactions? GetByUserAndVideo(int userId, string videoToken)
        {
            return DbSet
                .Where(e => e.UserId == userId && e.Video.Token == videoToken)
                .Select(e => e.Value)
                .FirstOrDefault()?
                .ParseEnum<Entity.Reactions>();
        }
    }
}
