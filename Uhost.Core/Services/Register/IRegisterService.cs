using System;
using System.Threading.Tasks;
using Uhost.Core.Models.User;

namespace Uhost.Core.Services.Register
{
    public interface IRegisterService : IDisposable
    {
        Task<bool> ConfirmRegistration(string code);
        Task RequestRegistrationAsync(UserRegisterModel model);
        Task SendRegistrationEmail(string key);
        bool UserExists(UserRegisterModel model);
    }
}
