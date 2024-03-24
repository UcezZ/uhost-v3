using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Uhost.Web.Common;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Уведомления
    /// </summary>
    [Route("api/v2/notifications"), Authorize]
    public class NotificationController : Controller
    {
        /// <summary>
        /// Получение всех непрочитанных уведомлений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            return ResponseHelper.Success(Enumerable.Empty<object>());
        }

        /// <summary>
        /// Получить полное тело уведомления и пометить его прочитанным
        /// </summary>
        /// <param name="id">ИД уведомления</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(string id)
        {
            return ResponseHelper.Success();
        }

        /// <summary>
        /// Получение количества непрочитанных уведомлений
        /// </summary>
        /// <returns></returns>
        [HttpGet("count")]
        public IActionResult GetCount()
        {
            return ResponseHelper.Success(0);
        }

        /// <summary>
        /// Пометить все уведомления прочитанными
        /// </summary>
        /// <returns></returns>
        [HttpPost("mark-read-all")]
        public IActionResult MarkReadAll()
        {
            return ResponseHelper.Success();
        }
    }
}
