using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.User;
using Uhost.Web.Attributes;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using Uhost.Web.Services.Auth;
using static Uhost.Core.Data.Entities.Log;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Авторизация и аутентификация
    /// </summary>
    [Route("api/v2/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ILogService _logService;

        public AuthController(IAuthService authService, IUserService userService, ILogService logService)
        {
            _authService = authService;
            _userService = userService;
            _logService = logService;
        }

        /// <summary>
        /// Проверка авторизации
        /// </summary>
        [HttpGet("check"), Authorize]
        public IActionResult OnlyAuthUsers()
        {
            return StatusCode(200);
        }

        /// <summary>
        /// Получаем информацию о пользователе по токену
        /// </summary>
        [HttpGet("info"), Authorize]
        public IActionResult Info()
        {
            if (!User.TryGetUserId(out var userId))
            {
                return ResponseHelper.ErrorMessage("id", ApiStrings.Auth_Error_Unauthorized);
            }

            return ResponseHelper.Success(_userService.GetOne(userId));
        }

        /// <summary>
        /// Выход пользователя (инвалидация токена)
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout"), Authorize]
        public IActionResult Logout()
        {
            if (_authService.Logout(User))
            {
            }

            return ResponseHelper.Success("OK");
        }

        /// <summary>
        /// Вход пользователя
        /// </summary>
        [HttpPost("login"), AllowAnonymous, Throttle(Count = 5, SpanSeconds = 10)]
        public IActionResult Login([FromForm] UserLoginQueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var user = _userService.Auth(query);

            if (user == null)
            {
                _logService.Log(
                     Events.UserAuthFail,
                     new
                     {
                         Request = query,
                         User = user?.ToModel<UserEntity, UserShortViewModel>()
                     });

                return ResponseHelper.ErrorMessage(nameof(query.Login), ApiStrings.Auth_Error_InvalidCredentials);
            }
            if (user.BlockedAt != null || user.BlockedByUser != null)
            {
                var comment = ApiStrings.Auth_Error_BlockFmt.Format(
                    user.Login,
                    user.BlockedAt?.ToHumanFmt(),
                    user.BlockedByUser?.Login,
                    user.BlockReason);

                _logService.Log(
                     Events.UserAuthFail,
                     new
                     {
                         Request = query,
                         User = user?.ToModel<UserEntity, UserShortViewModel>(),
                         Comment = comment
                     });

                return ResponseHelper.ErrorMessage("login", comment);
            }

            _authService.GenToken(user.Id, out var expiresAt, out var token);
            var model = _userService.GetOne(user.Id);
            _userService.UpdateLastVisitAt(user.Id);
            _logService.Log(Events.UserAuth, user?.ToModel<UserEntity, UserShortViewModel>());

            return ResponseHelper.Success(new
            {
                Token = token,
                ValidTo = expiresAt?.ToApiFmt(),
                User = model,
            });
        }
    }
}
