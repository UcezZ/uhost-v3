using System;
using System.Threading.Tasks;
using Uhost.Core.Models.User;

namespace Uhost.Core.Services.Register
{
    public interface IRegisterService : IDisposable
    {
        Task<UserViewModel> ConfirmRegistration(string code);
        Task<bool> RequestRegistrationAsync(UserRegisterModel model);
        bool UserExists(UserRegisterModel model);
    }
}
