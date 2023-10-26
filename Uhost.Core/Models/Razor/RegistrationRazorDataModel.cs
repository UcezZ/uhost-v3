using Uhost.Core.Models.User;

namespace Uhost.Core.Models.Razor
{
    public class RegistrationRazorDataModel
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public UserRegisterModel Model { get; set; }
    }
}
