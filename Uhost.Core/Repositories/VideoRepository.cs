using Microsoft.EntityFrameworkCore;
using System;
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
            if (!string.IsNullOrEmpty(query.Token))
            {
                q = q.Where(e => e.Token == query.Token);
            }
            if (query.UserId > 0)
            {
                q = q.Where(e => e.UserId == query.UserId);
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }
            if (query.AllowComments is bool allowComments)
            {
                q = q.Where(e => e.AllowComments == allowComments);
            }
            if (query.AllowReactions is bool allowReactions)
            {
                q = q.Where(e => e.AllowReactions == allowReactions);
            }
            if (!query.ShowHidden)
            {
                q = q.Where(e => !e.IsHidden);
            }
            if (!query.ShowPrivate)
            {
                q = q.Where(e => !e.IsPrivate);
            }
            if (query.IncludeProcessingStates)
            {
                q = q.Include(e => e.VideoProcessingStates)
                    .Where(e => e.VideoProcessingStates.Any(e => e.DeletedAt == null));
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                q = q.Where(e => PostgreSqlFunctions.TrgmAreSimilar(PostgreSqlFunctions.Debloat(e.Name), PostgreSqlFunctions.Debloat(query.Name))
                || EF.Functions.Like(PostgreSqlFunctions.Debloat(e.Name), "%" + PostgreSqlFunctions.Debloat(query.Name) + "%"));

                if (query.IsDescending)
                {
                    q = q.OrderBy(e => PostgreSqlFunctions.TrgmWordSimilarity(PostgreSqlFunctions.Debloat(e.Name), PostgreSqlFunctions.Debloat(query.Name)))
                        .ThenByDescending(e => e.Name)
                        .ThenByDescending(e => e.CreatedAt);
                }
                else
                {
                    q = q.OrderByDescending(e => PostgreSqlFunctions.TrgmWordSimilarity(PostgreSqlFunctions.Debloat(e.Name), PostgreSqlFunctions.Debloat(query.Name)))
                        .ThenBy(e => e.Name)
                        .ThenBy(e => e.CreatedAt);
                }
            }
            else
            {
                if (query.SortByParsed == Entity.SortBy.Random)
                {
                    q = q.OrderBy(e => PostgreSqlFunctions.Random());
                }
                else
                {
                    q = q.OrderBy(query);
                }
            }

            return q;
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null) where TModel : BaseModel<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }

        public (string Token, TimeSpan Duration) GetTokenAndDuration(int id)
        {
            var obj = DbSet
                .Where(e => e.Id == id && e.DeletedAt == null)
                .Select(e => new { e.Token, e.Duration })
                .FirstOrDefault();

            return (obj.Token, obj.Duration);
        }

        public int GetId(string token)
        {
            return DbSet
                .Where(e => e.Token == token && e.DeletedAt == null)
                .Select(e => e.Id)
                .FirstOrDefault();
        }

        internal void UpdateDuration(int id, TimeSpan duration)
        {
            if (FindEntity(id, out var entity))
            {
                entity.Duration = duration;
                Save();
            }
        }
    }
}
