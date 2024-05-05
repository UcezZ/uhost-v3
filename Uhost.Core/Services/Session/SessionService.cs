using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.File;
using Uhost.Core.Models.Session;
using Uhost.Core.Models.User;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.RedisSwitcher;
using Uhost.Core.Services.Token;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Log;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Services.Session
{
    public sealed class SessionService : BaseService, ISessionService
    {
        private readonly IFileService _files;
        private readonly IRedisSwitcherService _redis;
        private readonly ILogService _log;
        private readonly UserRepository _users;

        public SessionService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IServiceProvider provider,
            IFileService files,
            IRedisSwitcherService redis,
            ILogService log) : base(factory, provider)
        {
            _files = files;
            _users = new UserRepository(_dbContext);
            _redis = redis;
            _log = log;
        }

        private static async Task<SessionViewModel> GetSessionFromRedisAsync(IDatabase redis, RedisKey key)
        {
            var keyInfo = new RedisKeyInfo(key, await redis.KeyTimeToLiveAsync(key));
            var payload = await redis.StringGetAsync(key);

            var model = new SessionViewModel(keyInfo);

            if (payload.TryCastTo<SessionClientInfoModel>(out var clientInfo))
            {
                model.ClientInfo = clientInfo;
            }

            return model;
        }

        public async Task<IQueryable<SessionViewModel>> GetAll(SessionQueryModel query)
        {
            var pattern = TokenService.GetRedisAuthKey(query.UserId > 0 ? query.UserId : "*", "*");

            var keys = await _redis.ExecuteAsync(async e => await e.Multiplexer.GetKeysAsync(pattern).ToListAsync());

            var models = await Task.WhenAll(keys.Select(e => _redis.ExecuteAsync(async r => await GetSessionFromRedisAsync(r, e))));

            var q = models
                .AsQueryable()
                .OrderBy(query);

            return q;
        }

        public async Task<PagerResultModel<SessionViewModel>> GetAllPaged(SessionQueryModel query)
        {
            var sessions = await GetAll(query);
            var pager = sessions.CreatePager(query);

            if (pager.Any())
            {
                var userQuery = new UserQueryModel
                {
                    Ids = pager.Select(e => e.UserId),
                    IncludeDeleted = true
                };
                var users = _users.GetAll<UserShortViewModel>(userQuery).ToList();
                var files = _files
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.UserId), typeof(UserEntity), FileTypes.UserAvatar)
                    .ToList();

                foreach (var model in users)
                {
                    model.AvatarUrl = files.FirstOrDefault(e => e.DynId == model.Id)?.Url;
                }

                TryGetUserId(out var userId);
                TryGetUserJti(out var jti);

                foreach (var model in pager)
                {
                    model.IsCurrent = model.UserId == userId && model.SessionGuid.Equals(jti, StringComparison.InvariantCultureIgnoreCase);
                    model.User = users.FirstOrDefault(e => e.Id == model.UserId);
                }
            }

            return pager.Paginate();
        }

        public async Task<bool> Terminate(Guid sessionGuid)
        {
            var pattern = TokenService.GetRedisAuthKey("*", sessionGuid);
            var keys = await _redis.ExecuteAsync(async e => await e.Multiplexer.GetKeysAsync(pattern).ToListAsync());

            if (!keys.Any())
            {
                return false;
            }

            var key = keys.First();
            var keyInfo = await _redis.ExecuteAsync(async e => new RedisKeyInfo(key, await e.KeyTimeToLiveAsync(key)));

            var result = await _redis.ExecuteAsync(async e => await e.KeyDeleteAsync(key));

            if (result)
            {
                _log.Add(Events.SessionTerminated, new
                {
                    Session = new SessionViewModel(keyInfo)
                });
            }

            return result;
        }
    }
}
