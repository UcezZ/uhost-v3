using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.Log;
using Uhost.Core.Models.User;
using Uhost.Core.Repositories;
using static System.Console;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Log
{
    public sealed class LogService : BaseService, ILogService
    {
        private readonly LogRepository _repo;
        private readonly UserRepository _users;

        public LogService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IDbContextFactory<PostgreSqlLogDbContext> logFactory,
            IServiceProvider provider) : base(factory, provider)
        {
            _repo = new LogRepository(logFactory.CreateDbContext());
            _users = new UserRepository(_dbContext);
        }

        public void Add(Events ev, object data = null, int userId = default, bool writeToStdOut = false)
        {
            if (userId <= 0)
            {
                TryGetUserId(out userId);
            }

            var entity = new Entity
            {
                UserId = userId > 0 ? userId : null,
                EventId = (int)ev,
                IPAddress = _httpContextAccessor?.HttpContext?.ResolveClientIp(),
                Data = (data ?? new { }).ToJson()
            };

            _repo.Add(entity);

            if (writeToStdOut)
            {
                WriteLine($"{ev.Translate()}\r\n{data?.ToJson(Formatting.Indented)}");
            }
        }

        public PagerResultModel<LogViewModel> GetAllPaged(LogQueryModel query)
        {
            var pager = _repo
                .GetAll<LogViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var userQuery = new UserQueryModel
                {
                    Ids = pager.Select(e => e.UserId),
                    IncludeDeleted = true
                };
                var users = _users
                    .GetAll<UserLogViewModel>(userQuery)
                    .ToList();

                foreach (var model in pager)
                {
                    model.User = users.FirstOrDefault(e => e.Id == model.UserId);
                }
            }

            return pager.Paginate();
        }
    }
}
