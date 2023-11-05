using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Uhost.Core.Services.User;
using Uhost.Tests.Extensions;
using Uhost.Web.Controllers;
using Uhost.Web.Services.Auth;
using Xunit;

namespace Uhost.Tests
{
    public sealed class AuthTest : BaseTest
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly AuthController _controller;

        public AuthTest() : base()
        {
            _userService = Provider.GetRequiredService<IUserService>();
            _authService = Provider.GetRequiredService<IAuthService>();
            _controller = Provider.Instantiate<AuthController>();
        }

        [Fact]
        public void Unauthorized()
        {
            _controller.ResetAuthorization();

            var result = _controller.Info();

            var jToken = result.CommonResponseAssertation(HttpStatusCode.Forbidden);
            var error = jToken.ErrorResponseAssertation();

            Assert.NotEmpty(error);
        }

        [Fact]
        public void Authorized()
        {
            var userModel = _userService.GetOne(1);
            var userClaims = _authService.CreateClaims(1);

            _controller.Authorize(userClaims);

            var result = _controller.Info();

            var jToken = result.CommonResponseAssertation();
            var data = jToken.SuccessResponseAssertation();
            var actualUser = data.ToObject<UserViewModel>();

            Assert.NotNull(actualUser);

            Assert.Equal(userModel.Id, actualUser.Id);
        }
    }
}
