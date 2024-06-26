﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Uhost.Core.Models.Comment;
using Uhost.Core.Services.Comment;
using Uhost.Web.Attributes;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using Entity = Uhost.Core.Data.Entities.Comment;
using VideoEntity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Комментарии
    /// </summary>
    [Route("api/v2/comments")]
    public class CommentController : Controller
    {
        private readonly ICommentService _service;

        public CommentController(ICommentService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение комментариев по токену видео
        /// </summary>
        /// <param name="videoToken"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("{videoToken}"), AllowAnonymous]
        public IActionResult GetAllByVideoPaged(
            [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string videoToken,
            [FromQuery]
            CommentShortQueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var fQuery = new CommentQueryModel
            {
                VideoToken = videoToken
            };

            fQuery.LoadFrom<PagedQueryModel>(query);

            return ResponseHelper.Success(_service.GetAllPaged(fQuery));
        }

        /// <summary>
        /// Создание комментария
        /// </summary>
        /// <param name="videoToken">Токен видео</param>
        /// <param name="text">Текст комментария</param>
        /// <returns></returns>
        [HttpPost("{videoToken}"), Authorize, Throttle(Count = 5, SpanSeconds = 30)]
        public IActionResult Create(
            [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string videoToken,
            [FromForm, StringLengthValidation(minLength: 3, maxLength: 512, allowEmpty: false, trim: false)]
            string text)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }
            if (!_service.CheckUserRestrictions(videoToken, out var missing))
            {
                return ResponseHelper.ErrorForbidden(missing);
            }
            if (!_service.AreCommentsAllowed(videoToken))
            {
                return ResponseHelper.Error(ApiStrings.Comment_Error_NotAllowed);
            }

            var entity = _service.Add(videoToken, text?.TrimAll());

            return ResponseHelper.Success(_service.GetOne(entity.Id));
        }

        /// <summary>
        /// Удаление комментария к видео
        /// </summary>
        /// <param name="videoToken">Токен видео</param>
        /// <param name="id">ИД комментария</param>
        /// <returns></returns>
        [HttpDelete("{videoToken}/{id}")]
        public IActionResult Delete(
            [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string videoToken,
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Comment_Error_NotFound))]
            string id)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Delete(videoToken, idParsed);

            return ResponseHelper.Success();
        }
    }
}
