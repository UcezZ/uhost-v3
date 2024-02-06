using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Uhost.Core.Extensions;

namespace Uhost.Web.Middleware
{
    public class DummyTokenHandler : ISecurityTokenValidator
    {
        private static readonly ClaimsPrincipal _emptyClaims = new ClaimsPrincipal();
        private static readonly SecurityToken _emptyToken;
        private static readonly JwtSecurityTokenHandler _tokenValidator;

        static DummyTokenHandler()
        {
            _tokenValidator = new JwtSecurityTokenHandler();
            _emptyToken = _tokenValidator.CreateToken(new SecurityTokenDescriptor());
        }

        public bool CanValidateToken => _tokenValidator.CanValidateToken;

        public int MaximumTokenSizeInBytes
        {
            get => _tokenValidator.MaximumTokenSizeInBytes;
            set => _tokenValidator.MaximumTokenSizeInBytes = value;
        }

        public bool CanReadToken(string securityToken) => _tokenValidator.CanReadToken(securityToken);

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            try
            {
                var claim = _tokenValidator.ValidateToken(securityToken, validationParameters, out validatedToken);

                if (claim == null || !claim.TryGetUserId(out var userId) || !claim.TryGetJti(out var jti))
                {
                    return _emptyClaims;
                }

                return claim;
            }
            catch
            {
                // оно всё ещё может свалиться, если вместо JWT в заголовке какая-нибудь гадость
                validatedToken = _emptyToken;
                return _emptyClaims;
            }
        }

        public string WriteToken(SecurityToken token) =>
            _tokenValidator.WriteToken(token);

        public SecurityToken CreateToken(SecurityTokenDescriptor tokenDescriptor) =>
            _tokenValidator.CreateToken(tokenDescriptor);
    }
}
