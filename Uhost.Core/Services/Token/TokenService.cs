using Newtonsoft.Json;
using System;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Core.Services.RedisSwitcher;

namespace Uhost.Core.Services.Token
{
    public sealed class TokenService : ITokenService
    {
        private const string _authKeyPrefix = "authtoken";
        private const string _regQueryKeyPrefix = "regquery";
        private const string _videoKeyPrefix = "videotoken";

        /// <summary>
        /// Регулярное выражения ключа авторизации
        /// </summary>
        public static Regex RedisAuthKeyRegex { get; } = new Regex(@$"^{_authKeyPrefix}_(\d+)_([0-9a-fA-F]{{8}}(\-[0-9a-fA-F]{{4}}){{4}}[0-9a-fA-F]{{8}})$");

        private readonly IRedisSwitcherService _redis;

        public TokenService(IRedisSwitcherService redis)
        {
            _redis = redis;
        }

        /// <summary>
        /// Ключ redis для данных авторизации
        /// </summary>
        /// <param name="id"></param>
        /// <param name="jti"></param>
        /// <returns></returns>
        public static string GetRedisAuthKey(object id, object jti)
        {
            return $"{_authKeyPrefix}_{id}_{jti}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetRedisRegisterQueryKey(string code, IPAddress ip)
        {
            return $"{_regQueryKeyPrefix}_{ip}_{code}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetRedisVideoKey(string key)
        {
            return $"{_videoKeyPrefix}_{key}";
        }

        /// <summary>
        /// Проверка, что ключ для токена существует
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<bool> CheckExistsAsync(ClaimsPrincipal claims)
        {
            if (!claims.TryGetUserId(out var userId) || !claims.TryGetJti(out var jti))
            {
                return false;
            }

            var key = GetRedisAuthKey(userId, jti);
            var exists = await _redis.ExecuteAsync(async e => await e.KeyExistsAsync(key));

            return exists;
        }

        /// <summary>
        /// Создаёт ключ для токена
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="jti"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task CreateAuthToken(int userId, string jti, TimeSpan expiry)
        {
            var key = GetRedisAuthKey(userId, jti);
            await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, string.Empty, expiry));
        }

        /// <summary>
        /// Инвалидация токена
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="jti"></param>
        /// <returns></returns>
        public async Task<bool> InvalidateAuthTokenAsync(int userId, string jti)
        {
            var key = GetRedisAuthKey(userId, jti);
            return await _redis.ExecuteAsync(async e => await e.KeyDeleteAsync(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="payload"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task StoreVideoKeyAsync(string key, object payload, TimeSpan expiry)
        {
            await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, payload.ToJson(Formatting.Indented), expiry));
        }
    }
}
