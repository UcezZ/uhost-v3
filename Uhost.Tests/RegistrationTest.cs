using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Models.User;
using Uhost.Core.Services.Register;
using Xunit;

namespace Uhost.Tests
{
    public sealed class RegistrationTest : BaseTest
    {
        private readonly IRegisterService _svc;

        public RegistrationTest()
        {
            _svc = Provider.GetRequiredService<IRegisterService>();
        }

        [Fact]
        public async Task SendEmailTest()
        {
            var model = new UserRegisterModel
            {
                Email = "ee@ma.il",
                Login = "log_in",
                Password = "ass-w0rD",
                PasswordConfirm = "ass-w0rD",
                Theme = nameof(Core.Data.Entities.User.Themes.Dark),
                Locale = nameof(Core.Data.Entities.User.Locales.Ru)
            };
            await _svc.RequestRegistrationAsync(model);

            Provider.RunHangfire(3000, TaskQueues.Distribution);
        }
    }
}
