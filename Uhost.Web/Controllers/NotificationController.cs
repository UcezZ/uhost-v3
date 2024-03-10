using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return Ok();
        }

        /// <summary>
        /// Получить полное тело уведомления и пометить его прочитанным
        /// </summary>
        /// <param name="id">ИД уведомления</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(string id)
        {
            return Ok();
        }

        /// <summary>
        /// Получение количества непрочитанных уведомлений
        /// </summary>
        /// <returns></returns>
        [HttpGet("count")]
        public IActionResult GetCount()
        {
            return Ok();
        }

        /// <summary>
        /// Пометить все уведомления прочитанными
        /// </summary>
        /// <returns></returns>
        [HttpPost("mark-read-all")]
        public IActionResult MarkReadAll()
        {
            return Ok();
        }
    }
}
