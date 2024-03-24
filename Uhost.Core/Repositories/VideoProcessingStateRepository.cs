using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.VideoProcessingState;
using Entity = Uhost.Core.Data.Entities.VideoProcessingState;
using QueryModel = Uhost.Core.Models.VideoProcessingState.VideoProcessingStateQueryModel;

namespace Uhost.Core.Repositories
{
    public class VideoProcessingStateRepository : BaseRepository<Entity>
    {
        public VideoProcessingStateRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

        public IQueryable<Entity> PrepareQuery(QueryModel query)
        {
            IQueryable<Entity> q = DbSet;

            if (query.Id > 0)
            {
                q = q.Where(e => e.Id == query.Id);
            }
            if (query.VideoId > 0)
            {
                q = q.Where(e => e.VideoId == query.VideoId);
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.Video.UserId == query.UserId);
            }
            if (query?.State != null)
            {
                q = q.Where(e => e.State == query.State.ToString());
            }
            if (!string.IsNullOrEmpty(query.Token))
            {
                q = q.Where(e => e.Video.Token == query.Token);
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }

            switch (query.SortByParsed)
            {
                case Entity.SortBy.VideoCreatedAt:
                    query.SortBy = $"{nameof(Entity.Video)}.{nameof(Entity.Video.CreatedAt)}";
                    break;
                case Entity.SortBy.UserId:
                    query.SortBy = $"{nameof(Entity.Video)}.{nameof(Entity.Video.UserId)}";
                    break;
            }

            return q.OrderBy(query);
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null) where TModel : BaseModel<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }

        /// <summary>
        /// Проверка, что все конвертации были завершены успешно
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        internal bool AreAllCompleted(int videoId)
        {
            var query = new QueryModel
            {
                VideoId = videoId
            };

            var q = PrepareQuery(query);

            var hasAny = q.Any();
            var allCompleted = q.All(e => e.State == nameof(Entity.VideoProcessingStates.Completed));

            return hasAny && allCompleted;
        }

        /// <summary>
        /// Обновляет статус конвертации
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        internal int UpdateState(int id, Entity.VideoProcessingStates state)
        {
            if (FindEntity(id, out var entity))
            {
                entity.State = state.ToString();
                return Save();
            }

            return 0;
        }

        /// <summary>
        /// Возвращает статусы конвертации видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        internal VideoProcessingStateProgressModel GetProgresses(string token)
        {
            var query = new QueryModel
            {
                Token = token
            };

            return GetCollection<VideoProcessingStateProgressModel>(PrepareQuery(query));
        }
    }
}
