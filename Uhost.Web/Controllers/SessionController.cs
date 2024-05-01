using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Models.Session;
using Uhost.Core.Services.Session;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.Right;
using static Uhost.Web.Common.RightRequirement;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Сессии пользователей
    /// </summary>
    [Route("api/v2/sessions"), Authorize]
    public sealed class SessionController : Controller
    {
        private readonly ISessionService _service;

        public SessionController(ISessionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение списка всех сессий
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet, HasRightAuthorize(CombinationRule.Or, Rights.AdminSessionAccess, Rights.AdminSessionTerminate)]
        public async Task<IActionResult> GetAll(SessionQueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var result = await _service.GetAllPaged(query);

            return ResponseHelper.Success(result);
        }

        /// <summary>
        /// Завершение сессии
        /// </summary>
        /// <param name="sessionGuid">GUID сессии</param>
        /// <remarks>
        /// GUID сессии должен соответствовать формату 8-4-4-4-12, например <c>01234567-89ab-cdef-0123-456789abcdef</c>
        /// </remarks>
        /// <returns></returns>
        [HttpDelete("{sessionGuid}"), HasRightAuthorize(Rights.AdminSessionTerminate)]
        public async Task<IActionResult> Terminate([RegExpValidation(@"^[0-9a-fA-F]{8}(\-[0-9a-fA-F]{4}){4}[0-9a-fA-F]{8}$", ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Common_Error_Invalid))] string sessionGuid)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var result = await _service.Terminate(Guid.Parse(sessionGuid));

            if (result)
            {
                return ResponseHelper.Success();
            }
            else
            {
                return ResponseHelper.Error(ApiStrings.Session_Error_NotFoundByGuid);
            }
        }
    }
}
