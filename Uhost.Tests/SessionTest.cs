using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Uhost.Core.Services.Session;
using Xunit;

namespace Uhost.Tests
{
    public sealed class SessionTest : BaseTest
    {
        private readonly ISessionService _svc;

        public SessionTest()
        {
            _svc = Provider.GetRequiredService<ISessionService>();
        }

        [Fact]
        public async Task GetSessionsTest()
        {
            var sessions = await _svc.GetAllPaged(new Core.Models.Session.SessionQueryModel());

            Assert.NotEmpty(sessions.Items);
        }
    }
}
