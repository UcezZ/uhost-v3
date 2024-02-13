using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return View("Index", WebSettings.HomePageConfig);
        }
    }
}
