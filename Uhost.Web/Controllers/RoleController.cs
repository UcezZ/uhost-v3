using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Role;
using Uhost.Core.Services.Role;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Role;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Роли пользователей
    /// </summary>
    [Route("api/v2/roles"), Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить список всех прав
        /// </summary>
        /// <returns></returns>
        [HttpGet("rights"), HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.RoleCreateUpdate, Rights.RoleDelete)]
        public IActionResult GetAllRights()
        {
            return ResponseHelper.Success(Enum.GetValues<Rights>().Select(e => e.ToString().ToCamelCase()));
        }

        /// <summary>
        /// Получить все роли по запросу
        /// </summary>
        [HttpGet, HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.RoleCreateUpdate, Rights.RoleDelete)]
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
        [HttpGet("{id}"), HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.RoleCreateUpdate, Rights.RoleDelete)]
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
        [HttpPost, HasRightAuthorize(Rights.RoleCreateUpdate)]
        public IActionResult Create([FromBody] RoleCreateModel model)
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
        [HttpPut("{id}"), HasRightAuthorize(Rights.RoleCreateUpdate)]
        public IActionResult Update(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Role_Error_NotFoundById))]
            string id,
            [FromBody] RoleCreateModel model)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
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
        [HttpDelete("{id}"), HasRightAuthorize(Rights.RoleDelete)]
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
    }
}
