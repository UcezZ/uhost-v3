using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Uhost.Core.Extensions;

namespace Uhost.Web.Middleware
{
    public class RedisTokenHandler : ISecurityTokenValidator
    {
        private const string _keyPrefix = "authtoken";
        private static readonly ClaimsPrincipal _emptyClaims = new ClaimsPrincipal();

        public static string RedisKey<TId, TJti>(TId id, TJti jti) =>
            $"{_keyPrefix}_{id}_{jti}";

        #region Wrap override
        public bool CanValidateToken => _tokenValidator.CanValidateToken;

        public int MaximumTokenSizeInBytes
        {
            get => _tokenValidator.MaximumTokenSizeInBytes;
            set => _tokenValidator.MaximumTokenSizeInBytes = value;
        }

        public bool CanReadToken(string securityToken) => _tokenValidator.CanReadToken(securityToken);
        #endregion

        private readonly JwtSecurityTokenHandler _tokenValidator;
        private readonly IRedisDatabase _redis;

        public RedisTokenHandler(IRedisClientFactory factory)
        {
            _tokenValidator = new JwtSecurityTokenHandler();
            _redis = factory.GetDefaultRedisDatabase();
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            try
            {
                var claim = _tokenValidator.ValidateToken(securityToken, validationParameters, out validatedToken);

                if (!claim.TryGetUserId(out var userId) || userId <= 0 || !claim.TryGetJti(out var jti))
                {
                    return _emptyClaims;
                }

                var key = RedisKey(userId, jti);
                var exists = _redis.ExistsAsync(key);
                exists.Wait();

                if (!exists.Result)
                {
                    return _emptyClaims;
                }

                return claim;
            }
            catch
            {
                validatedToken = _tokenValidator.CreateToken(new SecurityTokenDescriptor());
                return new ClaimsPrincipal();
            }
        }

        internal SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor) => _tokenValidator.CreateToken(tokenDescriptor);

        public string WriteToken(SecurityToken token)
        {
            var stringToken = _tokenValidator.WriteToken(token);
            if (token is JwtSecurityToken jwtToken &&
                jwtToken.Claims.TryGetUserId(out var id) &&
                jwtToken.Claims.TryGetJti(out var jti))
            {
                var key = RedisKey(id, jti);

                _redis
                    .Database
                    .StringSet(key, string.Empty, token.ValidTo - token.ValidFrom);

                return stringToken;
            }
            else
            {
                return null;
            }
        }

        public bool InvalidateToken(int userId, string jti)
        {
            var key = RedisKey(userId, jti);
            var result = _redis
                .Database
                .KeyDelete(key);

            return result;
        }
    }
}
