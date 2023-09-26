using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Xml;
using Uhost.Core.Extensions;

namespace Uhost.Core.Middleware
{
    public class RedisTokenHandler : SecurityTokenHandler
    {
        private const string _keyPrefix = "authtoken_";

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
                var jwtId = claim.FindFirst(c => c.Type == "Id")?.Value;
                var jti = claim.FindFirst(c => c.Type == "jti")?.Value ?? string.Empty;
                string redisId = _connectionMultiplexer.GetDatabase(CoreSettings.RedisDatabase).StringGet(_keyPrefix + jti);

                if (redisId == null || jwtId == null || jwtId != redisId || !claim.TryGetUserId(out var userId))
                {
                    return new ClaimsPrincipal();
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
                jwtToken.Claims.FirstOrDefault(e => e.Type == "Id")?.Value is string id &&
                jwtToken.Claims.FirstOrDefault(e => e.Type == "jti")?.Value is string jti)
            {
                _connectionMultiplexer.GetDatabase(CoreSettings.RedisDatabase).StringSet(_keyPrefix + jti, id, token.ValidTo - token.ValidFrom);
                return stringToken;
            }
            else
            {
                return null;
            }
        }

        public int InvalidateToken(string jti)
        {
            var result = _connectionMultiplexer.GetDatabase(CoreSettings.RedisDatabase).StringGetDelete(_keyPrefix + jti);

            return int.TryParse(result, out int num) ? num : 0;
        }
    }
}
