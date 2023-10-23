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
        /// Получить файл по ИД
        /// </summary>
        /// <param name="id">ИД файла</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.File_Error_NotFoundById))]
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

            return ResponseHelper.Success(_service.GetOne(idParsed));
        }

        /// <summary>
        /// Получает прогресс конвертации видео
        /// </summary>
        /// <param name="id">ИД загруженного видео</param>
        /// <returns></returns>
        [HttpGet("{id}/progress")]
        public async Task<IActionResult> Progress(string id)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }

            return ResponseHelper.Success(await _service.GetConversionProgress(idParsed));
        }

        /// <summary>
        /// Загрузка файла
        /// </summary>
        /// <param name="model">Модель данных</param>
        [HttpPost, HasRightAuthorize(Rights.VideoCreateUpdate), BigFileUpload]
        public IActionResult Create([FromForm] VideoCreateModel model)
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
        /// Удаление файла
        /// </summary>
        /// <param name="id">ИД файла</param>
        [HttpDelete("{id}"), HasRightAuthorize(Rights.VideoDelete)]
        public IActionResult Delete(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.File_Error_NotFoundById))]
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

            _service.Delete(idParsed);

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
