﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using static Uhost.Core.Data.Entities.Right;
using RoleRightEntity = Uhost.Core.Data.Entities.RoleRight;
using UserRoleEntity = Uhost.Core.Data.Entities.UserRole;

namespace Uhost.Core.Services
{
    public abstract class BaseService : IDisposable, IAsyncDisposable
    {
        private static readonly string _rightsSql = $@"SELECT DISTINCT
    rr.""{nameof(RoleRightEntity.RightId)}""
FROM
    ""{Tools.GetEntityTableName(typeof(UserRoleEntity))}"" ur
    INNER JOIN ""{Tools.GetEntityTableName(typeof(RoleRightEntity))}"" rr ON ur.""{nameof(UserRoleEntity.RoleId)}"" = rr.""{nameof(RoleRightEntity.RoleId)}""
WHERE
    ur.""{nameof(UserRoleEntity.UserId)}"" = @UserId
ORDER BY
    rr.""{nameof(RoleRightEntity.RightId)}""";
        protected readonly PostgreSqlDbContext _dbContext;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseService(PostgreSqlDbContext dbContext, IServiceProvider provider)
        {
            _dbContext = dbContext;
            _httpContextAccessor = provider.GetService<IHttpContextAccessor>();
        }

        protected bool TryGetUserId(out int userId)
        {
            userId = default;

            return _httpContextAccessor?.HttpContext?.User != null && _httpContextAccessor.HttpContext.User.TryGetUserId(out userId);
        }

        protected bool TryGetUserRights(out IReadOnlyCollection<Rights> rights)
        {
            if (_httpContextAccessor?.HttpContext?.User != null && _httpContextAccessor.HttpContext.User.TryGetUserId(out var userId))
            {
                rights = _dbContext.Database.FromSqlRaw<int>(_rightsSql, new NpgsqlParameter("UserId", userId))
                    .Select(e => (Rights)e)
                    .ToList();

                return true;
            }

            rights = Array.Empty<Rights>();

            return false;
        }

        public virtual void Dispose()
        {
            _dbContext.Dispose();
        }

        public virtual async ValueTask DisposeAsync()
        {
            await _dbContext.DisposeAsync();
        }
    }
}
