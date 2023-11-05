using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Web.Properties;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Контроллер по умолчанию
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        /// <summary>
        /// Ответ по умолчанию
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return Content(ApiStrings.Home_Index.Replace("%ORIGIN%", $"//{HttpContext.Request.Host.Host}"), "text/html");
        }
    }
}
