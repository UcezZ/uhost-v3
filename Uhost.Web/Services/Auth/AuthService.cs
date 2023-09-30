using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Services;
using Uhost.Web.Middleware;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Web.Services.Auth
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisTokenHandler _tokenHandler;

        public AuthService(PostgreSqlDbContext context, IConnectionMultiplexer connectionMultiplexer) : base(context)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _tokenHandler = new RedisTokenHandler(_connectionMultiplexer);
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
        /// <param name="expiresAt"></param>
        /// <param name="stringToken"></param>
        /// <returns></returns>
        public void GenToken(int userId, out DateTime? expiresAt, out string stringToken)
        {
            var tokenDescriptor = GenTokenDescriptor(userId);

            stringToken = _tokenHandler.WriteToken(_tokenHandler.CreateToken(tokenDescriptor));
            expiresAt = tokenDescriptor.Expires;
        }

        public bool Logout(ClaimsPrincipal claims)
        {
            if (claims.TryGetUserId(out var userId) && claims.TryGetJti(out var jti))
            {
                return claims != null && _tokenHandler.InvalidateToken(userId, jti) > 0;
            }

            return false;
        }
    }
}
