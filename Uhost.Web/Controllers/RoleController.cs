using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Role;
using Uhost.Core.Services.Role;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using Entity = Uhost.Core.Data.Entities.Role;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Роли пользователей
    /// </summary>
    [Route("api/v2/roles"), AllowAnonymous]
    public class RoleController : Controller, IDisposable, IAsyncDisposable
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить все роли по запросу
        /// </summary>
        [HttpGet]
        public IActionResult GetAll(RoleQueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        /// <summary>
        /// Получить роль по ИД
        /// </summary>
        /// <param name="id">ИД роли</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Role_Error_NotFoundById))]
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
        /// Создать роль
        /// </summary>
        /// <param name="model">Модель данных</param>
        [HttpPost]
        public IActionResult Create(RoleCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }
            if (_service.Exists(model.Name))
            {
                return ResponseHelper.ErrorMessage(nameof(model.Name), ApiStrings.Role_Error_AlreadyExists);
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
        /// Изменение роли
        /// </summary>
        /// <param name="id">ИД роли</param>
        /// <param name="model">Модель данных</param>
        [HttpPut("{id}")]
        public IActionResult Update(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Role_Error_NotFoundById))]
            string id,
            RoleCreateModel model)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }
            if (!_service.CheckRightIds(model.RightIds, out var invalid))
            {
                ModelState.AddModelError(nameof(model.RightIds), ApiStrings.Right_Error_NotFoundByIdFmt.Format(invalid));
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }
            if (_service.Exists(model.Name, idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(model.Name), ApiStrings.Role_Error_AlreadyExists);
            }

            _service.Update(idParsed, model);

            return ResponseHelper.Success(_service.GetOne(idParsed));
        }

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="id">ИД роли</param>
        [HttpDelete("{id}")]
        public IActionResult Delete(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Role_Error_NotFoundById))]
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
