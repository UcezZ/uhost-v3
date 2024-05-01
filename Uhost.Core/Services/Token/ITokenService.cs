using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Uhost.Core.Services.Token
{
    public interface ITokenService
    {
        Task<bool> CheckExistsAsync(ClaimsPrincipal claims);
        Task CreateAuthToken(int userId, string jti, TimeSpan expiry);
        Task<bool> InvalidateAuthTokenAsync(int userId, string jti);
        Task StoreVideoKeyAsync(string key, object payload, TimeSpan expiry);
    }
}
