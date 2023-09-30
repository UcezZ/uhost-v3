using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using Uhost.Core.Common;
using Uhost.Web.Properties;

namespace Uhost.Web.Controllers
{
    [Produces("application/json"), AllowAnonymous]
    public class ExceptionController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ExceptionController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("error"), ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            var ctx = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = ctx?.Error;

            if (_env.IsDevelopment() && exception != null)
            {
                var errorObj = new Dictionary<string, string>();
                var code = HttpStatusCode.InternalServerError;

                // TODO: дополнительная информация в зависимости от типа исключения (InnerException и т.д.)

                errorObj["error"] = exception.Message;
                errorObj["stackTrace"] = exception.StackTrace;
                errorObj["source"] = exception.Source;

                if (exception.InnerException != null)
                {
                    var e = exception.InnerException;

                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                    }

                    errorObj["innerMessage"] = e.Message;
                    errorObj["innerStackTrace"] = e.StackTrace;
                }

                return ResponseHelper.Error(errorObj, code);
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
