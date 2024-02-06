using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Uhost.Core.Extensions;

namespace Uhost.Core.Extensions
{
    public static class ClaimsExtensions
    {
        /// <summary>
        /// Получить ИД пользователя из <see cref="ClaimsPrincipal"/>
        /// </summary>
        public static bool TryGetUserId(this ClaimsPrincipal claims, out int userId)
        {
            if (claims != null)
            {
                return claims.Claims.TryGetUserId(out userId);
            }
            else
            {
                userId = default;

                return false;
            }
        }

        /// <summary>
        /// Получить GUID токена из <see cref="ClaimsPrincipal"/>
        /// </summary>
        public static bool TryGetJti(this ClaimsPrincipal claims, out string jti)
        {
            if (claims != null)
            {
                return claims.Claims.TryGetJti(out jti);
            }
            else
            {
                jti = default;

                return false;
            }
        }

        /// <summary>
        /// Получить ИД пользователя из <see cref="ClaimsPrincipal"/>
        /// </summary>
        public static bool TryGetUserId(this IEnumerable<Claim> claims, out int userId)
        {
            var c = claims?.FirstOrDefault(c => c.Type == nameof(Data.Entities.User.Id));

            if (c == null || !int.TryParse(c.Value, out userId))
            {
                userId = default;

                return false;
            }

            return userId > 0;
        }

        /// <summary>
        /// Получить GUID токена из <see cref="ClaimsPrincipal"/>
        /// </summary>
        public static bool TryGetJti(this IEnumerable<Claim> claims, out string jti)
        {
            var c = claims?.FirstOrDefault(c => c.Type == nameof(jti));

            jti = c?.Value;

            return !string.IsNullOrEmpty(jti);
        }
    }
}
