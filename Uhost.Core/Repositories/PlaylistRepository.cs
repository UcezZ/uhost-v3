using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Playlist;
using EntryEntity = Uhost.Core.Data.Entities.PlaylistEntry;
using QueryModel = Uhost.Core.Models.Playlist.PlaylistQueryModel;

namespace Uhost.Core.Repositories
{
    public class PlaylistRepository : BaseRepository<Entity>
    {
        protected override Func<IQueryable<Entity>, IQueryable<Entity>> DbSetUpdateTransformations => e => e
            .Include(e => e.PlaylistEntries)
            .ThenInclude(e => e.Video)
            .Include(e => e.User);

        public PlaylistRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet
                .AsNoTracking()
                .Include(e => e.User)
                .Include(e => e.PlaylistEntries)
                .ThenInclude(e => e.Video)
                .ThenInclude(e => e.User);

            if (query.Id > 0)
            {
                q = q.Where(e => e.Id == query.Id);
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.UserId == query.UserId);
            }
            if (!string.IsNullOrEmpty(query.UserLogin))
            {
                q = q.Where(e => e.User.Login == query.UserLogin);
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }

            if (query.SortByParsed == Entity.SortBy.VideoCount)
            {
                if (query.IsDescending)
                {
                    q = q.OrderByDescending(e => e.PlaylistEntries.Count);
                }
                else
                {
                    q = q.OrderBy(e => e.PlaylistEntries.Count);
                }
            }
            else
            {
                q = q.OrderBy(query);
            }

            return q;
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null) where TModel : IEntityLoadable<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }

        /// <summary>
        /// Добавление одного видео в конец плейлиста
        /// </summary>
        /// <param name="id"></param>
        /// <param name="videoId"></param>
        public void AddVideo(int id, int videoId)
        {
            if (FindEntity(id, out var entity))
            {
                var order = entity.PlaylistEntries.SafeMax(e => e.Order) + 1;

                entity.PlaylistEntries.Add(new EntryEntity
                {
                    VideoId = videoId,
                    Order = order
                });

                Save();
            }
        }
    }
}
