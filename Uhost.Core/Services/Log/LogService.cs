﻿using Microsoft.AspNetCore.Http;
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
    public class LogService : BaseService, ILogService
    {
        private readonly PostgreSqlLogDbContext _logContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly LogRepository _repo;

        public static IEnumerable<object> AllEvents { get; }

        static LogService()
        {
            AllEvents = Enum
                .GetValues<Events>()
                .Select(e => new { Id = (int)e, Name = e.ToString() });
        }

        public LogService(IServiceProvider provider, PostgreSqlDbContext dbContext, PostgreSqlLogDbContext logContext) : base(dbContext)
        {
            _repo = new LogRepository(logContext);
            _contextAccessor = provider.GetService<IHttpContextAccessor>();
        }

        public void Log(Events ev, object data)
        {
            var entity = new Entity
            {
                InvokerId = _contextAccessor?.HttpContext?.User != null && _contextAccessor.HttpContext.User.TryGetUserId(out var userId) ? userId : null,
                EventId = (int)ev,
                IPAddress = _contextAccessor?.HttpContext?.Connection?.RemoteIpAddress,
                Data = data?.ToJson()
            };

            _repo.Add(entity);
        }
    }
}
