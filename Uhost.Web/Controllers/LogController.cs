using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Core.Models.Log;
using Uhost.Core.Services.Log;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Web.Controllers
{
    [Route("api/v2/logs"), Authorize, HasRightAuthorize(Rights.AdminLogAccess)]
    public class LogController : Controller
    {
        private readonly ILogService _service;

        public LogController(ILogService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll(LogQueryModel query)
        {
            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        [HttpGet("events")]
        public IActionResult GetEvents()
        {
            return ResponseHelper.Success(_service.AllEvents);
        }
    }
}
