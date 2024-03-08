using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.VideoConversionState;
using Entity = Uhost.Core.Data.Entities.VideoConversionState;
using QueryModel = Uhost.Core.Models.VideoConversionState.VideoConversionStateQueryModel;

namespace Uhost.Core.Repositories
{
    public class VideoConversionStateRepository : BaseRepository<Entity>
    {
        public VideoConversionStateRepository(PostgreSqlDbContext dbContext) : base(dbContext) { }

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

            return q;
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
            var allCompleted = q.All(e => e.State == nameof(Entity.VideoConversionStates.Completed));

            return hasAny && allCompleted;
        }

        /// <summary>
        /// Обновляет статус конвертации
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        internal int UpdateState(int id, Entity.VideoConversionStates state)
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
        internal VideoConversionStateProgressModel GetProgresses(string token)
        {
            var query = new QueryModel
            {
                Token = token
            };

            return GetCollection<VideoConversionStateProgressModel>(PrepareQuery(query));
        }
    }
}
