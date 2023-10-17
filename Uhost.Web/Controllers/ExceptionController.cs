using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;
using Uhost.Core.Extensions;
using Uhost.Web.Common;
using Uhost.Web.Properties;

namespace Uhost.Web.Controllers
{
    [Produces("application/json")]
    public class ExceptionController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ExceptionController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("error"), ApiExplorerSettings(IgnoreApi = true), AllowAnonymous]
        public IActionResult Error()
        {
            var ctx = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = ctx?.Error;

            if (_env.IsDevelopment() && exception != null)
            {
                return ResponseHelper.Error(exception.ToDetailedDataObject(), HttpStatusCode.InternalServerError);
            }

            return ResponseHelper.ErrorMessage("error", ApiStrings.Common_Error_Common, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Бросает исключение
        /// </summary>
        /// <param name="message">Текст</param>
        /// <returns></returns>
        [HttpGet("api/v2/exception")]
        public IActionResult Exception(string message) =>
            throw new Exception(message);
    }
}
