using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Repositories;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Log
{
    public sealed class LogService : BaseService, ILogService
    {
        private readonly LogRepository _repo;

        public static IEnumerable<object> AllEvents { get; }

        static LogService()
        {
            AllEvents = Enum
                .GetValues<Events>()
                .Select(e => new { Id = (int)e, Name = e.ToString() });
        }

        public LogService(PostgreSqlDbContext dbContext, PostgreSqlLogDbContext logContext, IServiceProvider provider) : base(dbContext, provider)
        {
            _repo = new LogRepository(logContext);
        }

        public void Add(Events ev, object data = null)
        {
            var entity = new Entity
            {
                UserId = TryGetUserId(out var userId) ? userId : null,
                EventId = (int)ev,
                IPAddress = _httpContextAccessor?.HttpContext?.ResolveClientIp(),
                Data = (data ?? new { }).ToJson()
            };

            _repo.Add(entity);
        }
    }
}
