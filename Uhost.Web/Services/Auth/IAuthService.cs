using System;
using System.Security.Claims;

namespace Uhost.Web.Services.Auth
{
    public interface IAuthService
    {
        void GenToken(int userId, out DateTime? expiresAt, out string stringToken);
        bool Logout(ClaimsPrincipal claims);
    }
}
