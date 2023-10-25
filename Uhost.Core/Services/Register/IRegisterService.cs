using System.Threading.Tasks;
using Uhost.Core.Models.User;

namespace Uhost.Core.Services.Register
{
    public interface IRegisterService
    {
        Task<UserViewModel> ConfirmRegistration(string code);
        Task<bool> RequestRegistrationAsync(UserRegisterModel model);
    }
}
