using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Uhost.Web.Services.Auth
{
    public interface IAuthService
    {
        ClaimsPrincipal CreateClaims(int userId);
        Task<(DateTime Expires, string Token)> GenToken(int userId);
        Task<bool> Logout(ClaimsPrincipal claims);
    }
}
