using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml;
using Uhost.Core.Extensions;

namespace Uhost.Web.Middleware
{
    public class RedisTokenHandler : SecurityTokenHandler
    {
        private const string _keyPrefix = "authtoken";
        private static readonly ClaimsPrincipal _emptyClaims = new ClaimsPrincipal();

        public static string RedisKey<TId, TJti>(TId id, TJti jti) =>
            $"{_keyPrefix}_{id}_{jti}";

        #region Wrap override
        public override bool CanValidateToken => _tokenValidator.CanValidateToken;
        public override Type TokenType => _tokenValidator.TokenType;
        public override SecurityToken ReadToken(XmlReader reader, TokenValidationParameters validationParameters) => _tokenValidator.ReadToken(reader, validationParameters);
        public override SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor) => _tokenValidator.CreateToken(tokenDescriptor);
        public override void WriteToken(XmlWriter writer, SecurityToken token) => _tokenValidator.WriteToken(writer, token);
        public override SecurityToken ReadToken(string tokenString) => _tokenValidator.ReadToken(tokenString);
        public override bool CanReadToken(string tokenString) => _tokenValidator.CanReadToken(tokenString);
        #endregion

        private readonly JwtSecurityTokenHandler _tokenValidator;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisTokenHandler(IConnectionMultiplexer connectionMultiplexer)
        {
            _tokenValidator = new JwtSecurityTokenHandler();
            _connectionMultiplexer = connectionMultiplexer;
        }

        public override ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            try
            {
                var claim = _tokenValidator.ValidateToken(securityToken, validationParameters, out validatedToken);

                if (!claim.TryGetUserId(out var userId) || userId <= 0 || !claim.TryGetJti(out var jti))
                {
                    return _emptyClaims;
                }

                var key = RedisKey(userId, jti);
                var exists = _connectionMultiplexer.GetDatabase().KeyExists(key);

                if (!exists)
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

        public override string WriteToken(SecurityToken token)
        {
            var stringToken = _tokenValidator.WriteToken(token);
            if (token is JwtSecurityToken jwtToken &&
                jwtToken.Claims.TryGetUserId(out var id) &&
                jwtToken.Claims.TryGetJti(out var jti))
            {
                var key = RedisKey(id, jti);

                _connectionMultiplexer
                    .GetDatabase()
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
            var result = _connectionMultiplexer
                .GetDatabase()
                .KeyDelete(key);

            return result;
        }
    }
}
