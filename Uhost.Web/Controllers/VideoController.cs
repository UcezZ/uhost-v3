using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Video;
using Uhost.Core.Services.Video;
using Uhost.Web.Attributes;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Video;
using QueryModel = Uhost.Core.Models.Video.VideoQueryModel;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Видео
    /// </summary>
    [Route("api/v2/videos")]
    public class VideoController : Controller, IDisposable, IAsyncDisposable
    {
        private readonly IVideoService _service;

        public VideoController(IVideoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить все файлы по запросу
        /// </summary>
        [HttpGet, HasRightAuthorize(Rights.FileGet)]
        public IActionResult GetAll(QueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        /// <summary>
        /// Получить видео по токену
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <returns></returns>
        [HttpGet("{token}")]
        public IActionResult GetOne(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.GetOne(token));
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
        [HttpPost("url"), HasRightAuthorize(Rights.VideoCreateUpdate), BigFileUpload]
        public IActionResult UploadUrl([FromForm] VideoUploadUrlModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var entity = _service.Add(model, out var isInfinite);

            if (entity == null)
            {
                return ResponseHelper.Error(ApiStrings.Common_Error_Common);
            }
            else
            {
                return ResponseHelper.Success(_service.GetOne(entity.Id).Having(e => e.IsInfinite = isInfinite));
            }
        }

        /// <summary>
        /// Обновление описания видео
        /// </summary>
        /// <param name="token">Токен видео</param>
        /// <param name="model">Можель данных</param>
        /// <returns></returns>
        [HttpPut("{token}"), HasRightAuthorize(Rights.VideoCreateUpdate)]
        public IActionResult Update(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Token), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Video_Error_NotFound))]
            string token,
            [FromForm] VideoUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Update(token, model);

            return ResponseHelper.Success(_service.GetOne(token));
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

        protected override void Dispose(bool disposing)
        {
            _service?.Dispose();

            base.Dispose(disposing);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await _service.DisposeAsync();
        }
    }
}
