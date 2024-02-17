using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Graylog;
using Uhost.Core.Services.Graylog;
using Uhost.Web.Attributes;
using Uhost.Web.Common;
using Uhost.Web.Extensions;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Интеграция с Graylog
    /// </summary>
    [Route("api/v2/graylog"), AllowAnonymous, IgnoreIfVariableUndefined("BACKEND_USE_GRAYLOG")]
    public class GraylogController : Controller
    {
        private readonly IGraylogService _service;

        public GraylogController(IGraylogService service)
        {
            _service = service;
        }

        /// <summary>
        /// Запрашивает логи из GrayLog, возвращает файл CSV
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetCsvLogs(GraylogQueryModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var stream = _service.GetGraylogCsvStream(model);

            return new FileStreamResult(stream, "text/csv")
            {
                FileDownloadName = $"graylog-{DateTime.Now.ToFileFmt()}.csv",
                LastModified = DateTime.Now
            };

        }
    }
}
