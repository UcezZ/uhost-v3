using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Uhost.Core.Models.User;
using Uhost.Core.Services.Register;
using Uhost.Web.Attributes;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Регистрация
    /// </summary>
    [Route("api/v2/register"), AllowAnonymous]
    public class RegisterController : Controller
    {
        private readonly IRegisterService _service;

        public RegisterController(IRegisterService service)
        {
            _service = service;
        }

        /// <summary>
        /// Запросить регистрацию
        /// </summary>
        /// <param name="model">Модель данных</param>
        [HttpPost, Throttle(Count = 5, SpanSeconds = 60)]
        public async Task<IActionResult> RequestRegistration(UserRegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }
            if (_service.UserExists(model))
            {
                return ResponseHelper.ErrorMessage(nameof(model.Name), ApiStrings.User_Error_AlreadyExists);
            }

            if (await _service.RequestRegistrationAsync(model))
            {
                return ResponseHelper.Success(ApiStrings.Register_EmailSent);
            }
            else
            {
                return ResponseHelper.Error(ApiStrings.Register_Error_FailedToSend);
            }
        }

        /// <summary>
        /// Подтвердить регистрацию
        /// </summary>
        /// <param name="code">Код подтверждения</param>
        [HttpPost("{code}"), Throttle(Count = 5, SpanSeconds = 60)]
        public async Task<IActionResult> ConfirmRegistration([StringLength(16)] string code)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var model = await _service.ConfirmRegistration(code);

            if (model == null)
            {
                return ResponseHelper.Error(ApiStrings.Register_Error_ConfirmFail);
            }
            else
            {
                return ResponseHelper.Success(model);
            }
        }
    }
}
