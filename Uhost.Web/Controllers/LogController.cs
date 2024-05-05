using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Log;
using Uhost.Core.Services.Log;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using static Uhost.Core.Data.Entities.Log;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Логи
    /// </summary>
    [Route("api/v2/logs"), Authorize]
    public class LogController : Controller
    {
        private readonly ILogService _service;

        public LogController(ILogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение всех логов по запросу
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet, HasRightAuthorize(Rights.AdminLogAccess)]
        public IActionResult GetAll(LogQueryModel query)
        {
            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        /// <summary>
        /// Все события
        /// </summary>
        /// <returns></returns>
        [HttpGet("events"), HasRightAuthorize(Rights.AdminLogAccess)]
        public IActionResult GetEvents()
        {
            return ResponseHelper.Success(Enum.GetValues<Events>().Select(e => e.ToString().ToCamelCase()));
        }
    }
}
