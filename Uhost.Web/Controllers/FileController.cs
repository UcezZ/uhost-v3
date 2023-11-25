using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Models.File;
using Uhost.Core.Services.File;
using Uhost.Web.Attributes;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.File;
using QueryModel = Uhost.Core.Models.File.FileQueryModel;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Файлы
    /// </summary>
    [Route("api/v2/files"), Authorize]
    public class FileController : Controller, IDisposable, IAsyncDisposable
    {
        private readonly IFileService _service;

        public FileController(IFileService service)
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

            return ResponseHelper.Success(_service.GetAllPaged<FileShortViewModel>(query));
        }

        /// <summary>
        /// Получить файл по ИД
        /// </summary>
        /// <param name="id">ИД файла</param>
        /// <returns></returns>
        [HttpGet("{id}"), HasRightAuthorize(Rights.FileGet)]
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

            return ResponseHelper.Success(_service.GetOne<FileViewModel>(idParsed));
        }

        /// <summary>
        /// Загрузка файла
        /// </summary>
        /// <param name="model">Модель данных</param>
        [HttpPost, HasRightAuthorize(Rights.FileCreateUpdate), BigFileUpload]
        public IActionResult Create([FromForm] FileUploadModel model)
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
                return ResponseHelper.Success(_service.GetOne<FileViewModel>(entity.Id));
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        /// <param name="id">ИД файла</param>
        [HttpDelete("{id}"), HasRightAuthorize(Rights.FileDelete)]
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
