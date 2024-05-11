using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Services.Reaction;
using Uhost.Web.Attributes;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using Entity = Uhost.Core.Data.Entities.Reaction;
using VideoEntity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Реакции к видео
    /// </summary>
    [Route("api/v2/reactions")]
    public class ReactionController : Controller
    {
        private readonly IReactionService _service;

        public ReactionController(IReactionService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение количества реакций к видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        [HttpGet("{token}"), AllowAnonymous]
        public IActionResult GetReactionsStatsByVideo(
            [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }


            return ResponseHelper.Success(_service.GetOne(token));
        }

        /// <summary>
        /// Добавление реакции к видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <param name="reaction">Реакция. Допустимые значения <c>Like</c>, <c>Dislike</c></param>
        /// <returns></returns>
        [HttpPost("{token}/{reaction}"), Throttle]
        public IActionResult PostReaction(
            [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token,
            [EnumValidation(typeof(Entity.Reactions), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Reaction_Error_ParseFail))]
            string reaction)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.SetAndGetStats(token, Enum.Parse<Entity.Reactions>(reaction, true)));
        }

        /// <summary>
        /// Удаление реакции к видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        [HttpDelete("{token}"), Throttle]
        public IActionResult RemoveReaction(
            [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.RemoveAndGetStats(token));
        }
    }
}
