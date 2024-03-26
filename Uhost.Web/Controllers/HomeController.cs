using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Web.Common;
using Uhost.Web.Properties;

namespace Uhost.Web.Controllers
{
#if DEBUG
    /// <summary>
    /// Контроллер по умолчанию
    /// </summary>
    [AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        private static readonly byte[] _stylesheet = Resources.SwaggerStylesheet;

        /// <summary>
        /// Дашборд API
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index", WebSettings.HomePageConfig);
        }

        /// <summary>
        /// Стили swagger
        /// </summary>
        /// <returns></returns>
        [HttpGet("swagger/style.css")]
        public IActionResult SwaggerStyles()
        {
            return new DisposableFileStreamResult(_stylesheet, "text/css");
        }
    }
#endif
}
