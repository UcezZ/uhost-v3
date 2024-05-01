using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.Token;
using Uhost.Web.Middleware;
using static Uhost.Core.Data.Entities.Log;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Web.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokens;
        private readonly ILogService _log;
        private readonly DummyTokenHandler _tokenHandler;

        public AuthService(ITokenService tokens, ILogService log)
        {
            _tokens = tokens;
            _tokenHandler = new DummyTokenHandler();
            _log = log;
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
                await _tokens.CreateAuthToken(id, jti, secToken.ValidTo - secToken.ValidFrom);

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
                var result = await _tokens.InvalidateAuthTokenAsync(userId, jti);

                if (result)
                {
                    _log.Add(Events.UserLogOut, userId: userId);
                }

                return result;
            }

            return false;
        }
    }
}
