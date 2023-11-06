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
            if (!string.IsNullOrEmpty(query.Name))
            {
                q = q.Where(e => PostgreSqlFunctions.TrgmAreSimilar(e.Name, query.Name) || EF.Functions.ILike(e.Name, $"%{query.Name.PostgresEscape()}%", "\\"));
            }
            if (!query.IncludeDeleted)
            {
                q = q.Where(e => e.DeletedAt == null);
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                if (query.SortDirectParsed == BaseQueryModel.SortDirections.Desc)
                {
                    q = q.OrderBy(e => PostgreSqlFunctions.TrgmWordSimilarity(e.Name, query.Name))
                        .ThenByDescending(e => e.Name)
                        .ThenByDescending(e => e.CreatedAt);
                }
                else
                {
                    q = q.OrderByDescending(e => PostgreSqlFunctions.TrgmWordSimilarity(e.Name, query.Name))
                        .ThenBy(e => e.Name)
                        .ThenBy(e => e.CreatedAt);
                }
            }
            else
            {
                q = q.OrderBy(query);
            }

            return q;
        }

        public IQueryable<TModel> GetAll<TModel>(QueryModel query = null) where TModel : BaseModel<Entity>, new()
        {
            query ??= new QueryModel();

            return Get<TModel>(PrepareQuery(query));
        }

        public string GetToken(int id)
        {
            return DbSet
                .Where(e => e.Id == id && e.DeletedAt == null)
                .Select(e => e.Token)
                .FirstOrDefault();
        }

        public TimeSpan GetDuration(int id)
        {
            return DbSet
                .Where(e => e.Id == id && e.DeletedAt == null)
                .Select(e => e.Duration)
                .FirstOrDefault();
        }
    }
}
