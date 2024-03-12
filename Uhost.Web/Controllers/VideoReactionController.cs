using Microsoft.AspNetCore.Mvc;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Реакции к видео
    /// </summary>
    [Route("api/v2/reactions")]
    public class VideoReactionController : Controller
    {
        /// <summary>
        /// Получение сводки по реакциямк видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        [HttpGet("{token}/stats")]
        public IActionResult GetReactionsStatsByVideo(string token)
        {
            return Ok();
        }

        /// <summary>
        /// Добавление реакции к видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <param name="reaction">Реакция. Допустимые значения <c>Like</c>, <c>Dislike</c></param>
        /// <returns></returns>
        [HttpPost("{token}/{reaction}")]
        public IActionResult PostReaction(string token, string reaction)
        {
            return Ok();
        }

        /// <summary>
        /// Удаление реакции к видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        [HttpDelete("{token}")]
        public IActionResult RemoveReaction(string token)
        {
            return Ok();
        }
    }
}
