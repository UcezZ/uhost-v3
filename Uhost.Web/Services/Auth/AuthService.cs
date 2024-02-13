using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Core.Services.RedisSwitcher;
using Uhost.Web.Middleware;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Web.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IRedisSwitcherService _redis;
        private readonly DummyTokenHandler _tokenHandler;

        public AuthService(IRedisSwitcherService redis)
        {
            _redis = redis;
            _tokenHandler = new DummyTokenHandler();
        }

        /// <summary>
        /// Генерация токена, получение дескриптора
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static SecurityTokenDescriptor GenTokenDescriptor(int id)
        {
            var expiresAt = DateTime.Now.AddMinutes(WebSettings.AuthTokenTtlMinutes);

            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(nameof(UserEntity.Id), id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = expiresAt,
                Issuer = WebSettings.Jwt.Issuer,
                Audience = WebSettings.Jwt.Audience,
                SigningCredentials = new SigningCredentials(WebSettings.Jwt.SecurityKey, SecurityAlgorithms.HmacSha512Signature)
            };
        }

        /// <summary>
        /// Генерация токена пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<(DateTime Expires, string Token)> GenToken(int userId)
        {
            var tokenDescriptor = GenTokenDescriptor(userId);
            var secToken = _tokenHandler.CreateToken(tokenDescriptor);
            if (secToken is JwtSecurityToken jwtToken &&
                jwtToken.Claims.TryGetUserId(out var id) &&
                jwtToken.Claims.TryGetJti(out var jti))
            {
                var key = RedisTokenHandlerMiddleware.RedisKey(id, jti);
                await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, string.Empty, secToken.ValidTo - secToken.ValidFrom));

                return (tokenDescriptor.Expires ?? default, _tokenHandler.WriteToken(secToken));
            }

            return default;
        }

        /// <summary>
        /// Создание <see cref="ClaimsPrincipal"/> для пользователя
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ClaimsPrincipal CreateClaims(int userId)
        {
            var tokenDescriptor = GenTokenDescriptor(userId);
            var secToken = _tokenHandler.CreateToken(tokenDescriptor);
            if (secToken is JwtSecurityToken jwtToken &&
                jwtToken.Claims.TryGetUserId(out _) &&
                jwtToken.Claims.TryGetJti(out _))
            {
                return new ClaimsPrincipal(tokenDescriptor.Subject);
            }

            return default;
        }

        /// <summary>
        /// Инвалидация токена
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public async Task<bool> Logout(ClaimsPrincipal claims)
        {
            if (claims != null && claims.TryGetUserId(out var userId) && claims.TryGetJti(out var jti))
            {
                var key = RedisTokenHandlerMiddleware.RedisKey(userId, jti);
                return await _redis.ExecuteAsync(async e => await e.KeyDeleteAsync(key));
            }

            return false;
        }
    }
}
