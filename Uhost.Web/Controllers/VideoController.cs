using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Video;
using Uhost.Core.Services.Video;
using Uhost.Web.Attributes;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Video;
using QueryModel = Uhost.Core.Models.Video.VideoQueryModel;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Видео
    /// </summary>
    [Route("api/v2/videos")]
    public class VideoController : Controller
    {
        private readonly IVideoService _service;

        public VideoController(IVideoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить все видео по запросу
        /// </summary>
        [HttpGet, AllowAnonymous]
        public IActionResult GetAll(QueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.OverrideByUserRestrictions(query);

            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        /// <summary>
        /// Получить случайные видео 
        /// </summary>
        /// <param name="count">Количество</param>
        [HttpGet("random"), AllowAnonymous]
        public IActionResult GetRandom(int? count)
        {
            return ResponseHelper.Success(_service.GetRandom(count > 0 ? (int)count : 16));
        }

        /// <summary>
        /// Получить видео по токену
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        [HttpGet("{token}"), AllowAnonymous]
        public async Task<IActionResult> GetOne(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var model = await _service.GetOne(token);

            var cookieOptions = new CookieOptions
            {
                IsEssential = true,
                Expires = DateTimeOffset.Now.Add(model.GetCookieTtl()),
                SameSite = LocalEnvironment.IsDev
                    ? SameSiteMode.None
                    : SameSiteMode.Strict
            };

            Response.Cookies.Append("video_token", model.GetAccessToken(), cookieOptions);

            return ResponseHelper.Success(model);
        }

        /// <summary>
        /// Скачать видео по токену
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <param name="type">Тип файла</param>
        /// <returns></returns>
        [HttpGet("{token}/download/{type}"), AllowAnonymous]
        public IActionResult Download(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token,
            [EnumValidation(typeof(FileTypes), whitelist:new[]
            {
                nameof(FileTypes.Video240p),
                nameof(FileTypes.Video480p),
                nameof(FileTypes.Video720p),
                nameof(FileTypes.Video1080p)
            }, ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.File_Error_TypeFail))]
            string type)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            if (_service.TryGetDownload(token, Enum.Parse<FileTypes>(type), out var name, out var stream, out var lastModified))
            {
                return new DisposableFileStreamResult(stream, name.GetContentType())
                {
                    FileDownloadName = name,
                    LastModified = lastModified
                };
            }
            else
            {
                return ResponseHelper.Error(ApiStrings.Video_Error_NotFound);
            }
        }

        /// <summary>
        /// Получает прогресс конвертации видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        [HttpGet("{token}/progress")]
        public async Task<IActionResult> Progress(string token)
        {
            return ResponseHelper.Success(await _service.GetConversionProgressAsync(token));
        }

        /// <summary>
        /// Загрузка файла видео
        /// </summary>
        /// <param name="model">Модель данных</param>
        [HttpPost, HasRightAuthorize(Rights.VideoCreateUpdate), BigFileUpload]
        public IActionResult UploadFile([FromForm] VideoUploadFileModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var entity = _service.Add(model);

            if (entity == null)
            {
                return ResponseHelper.Error(ApiStrings.Common_Error_Common);
            }
            else
            {
                return ResponseHelper.Success(_service.GetOne(entity.Id));
            }
        }

        /// <summary>
        /// Загрузка видео из потока или стороннего сервера
        /// </summary>
        /// <param name="model">Модель данных</param>
        [HttpPost("url"), HasRightAuthorize(Rights.VideoCreateUpdate)]
        public IActionResult UploadUrl([FromForm] VideoUploadUrlModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var entity = _service.Add(model);

            if (entity == null)
            {
                return ResponseHelper.Error(ApiStrings.Common_Error_Common);
            }
            else
            {
                return ResponseHelper.Success(_service.GetOne(entity.Id));
            }
        }

        /// <summary>
        /// Обновление описания видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <param name="model">Можель данных</param>
        /// <returns></returns>
        [HttpPut("{token}"), HasRightAuthorize(Rights.VideoCreateUpdate)]
        public async Task<IActionResult> Update(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token,
            [FromForm] VideoUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Update(token, model);

            var updated = await _service.GetOne(token);

            var cookieOptions = new CookieOptions
            {
                IsEssential = true,
                Expires = new DateTimeOffset().Add(updated.GetCookieTtl()),
                SameSite = LocalEnvironment.IsDev
                    ? SameSiteMode.None
                    : SameSiteMode.Strict
            };

            Response.Cookies.Append("video_token", updated.GetAccessToken(), cookieOptions);

            return ResponseHelper.Success(updated);
        }

        /// <summary>
        /// Удаление видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        [HttpDelete("{token}"), HasRightAuthorize(Rights.VideoDelete)]
        public IActionResult Delete(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Delete(token);

            return ResponseHelper.Success();
        }
    }
}
