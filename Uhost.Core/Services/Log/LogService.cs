using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Repositories;
using static System.Console;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Log
{
    public sealed class LogService : BaseService, ILogService
    {
        private readonly LogRepository _repo;
        private readonly static IDictionary<Events, string> _allEvents;

        public IDictionary<Events, string> AllEvents => _allEvents;

        static LogService()
        {
            _allEvents = Enum
                .GetValues<Events>()
                .ToDictionary(e => e, e => e.Translate());
        }

        public LogService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IDbContextFactory<PostgreSqlLogDbContext> logFactory,
            IServiceProvider provider) : base(factory, provider)
        {
            _repo = new LogRepository(logFactory.CreateDbContext());
        }

        public void Add(Events ev, object data = null, bool writeToStdOut = false)
        {
            var entity = new Entity
            {
                UserId = TryGetUserId(out var userId) ? userId : null,
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
    }
}
