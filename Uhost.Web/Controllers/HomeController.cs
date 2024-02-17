using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Web.Properties;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Контроллер по умолчанию
    /// </summary>
    [AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        /// <summary>
        /// Ответ по умолчанию
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View("Index", WebSettings.HomePageConfig);
        }

        [HttpGet("swagger/style.css")]
        public IActionResult SwaggerStyles()
        {
            return Content(ApiStrings.SwaggerStylesheet, "text/css");
        }
    }
}
