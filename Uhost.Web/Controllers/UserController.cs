using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Uhost.Core.Services.User;
using Uhost.Web.Attributes.Authorize;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Пользователи
    /// </summary>
    [Route("api/v2/users"), Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получить всех пользователей по запросу
        /// </summary>
        [HttpGet, HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.UserCreate, Rights.UserDelete, Rights.UserInteractAll)]
        public IActionResult GetAll(UserQueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        /// <summary>
        /// Получить пользователя по ИД
        /// </summary>
        /// <param name="id">ИД пользователя</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetOne(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.User_Error_NotFoundById))]
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
        /// Получение пользователя по логину
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns></returns>
        [HttpGet("by-login/{login}"), AllowAnonymous]
        public IActionResult GetOneByLogin(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Login), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.User_Error_NotFoundById))]
            string login)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.GetOne(login));
        }

        /// <summary>
        /// Создать пользователя (для админа)
        /// </summary>
        /// <param name="model">Модель данных</param>
        [HttpPost, HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.UserCreate, Rights.UserInteractAll)]
        public IActionResult Create([FromForm] UserCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }
            if (_service.Exists(model.Name, model.Login))
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
        /// Изменение пользователя (для админа)
        /// </summary>
        /// <param name="id">ИД пользователя</param>
        /// <param name="model">Модель данных</param>
        [HttpPut("{id}"), HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.UserCreate, Rights.UserInteractAll)]
        public IActionResult Update(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.User_Error_NotFoundById))]
            string id,
           [FromForm] UserCreateModel model)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }
            if (_service.Exists(model.Name, model.Login, idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(model.Name), ApiStrings.User_Error_AlreadyExists);
            }

            _service.Update(idParsed, model);

            return ResponseHelper.Success(_service.GetOne(idParsed));
        }

        /// <summary>
        /// Смена пароля пользователя (для админа)
        /// </summary>
        /// <param name="id">ИД пользователя</param>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        [HttpPut("password/{id}"), HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.UserInteractAll)]
        public IActionResult UpdatePassword(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Role_Error_NotFoundById))]
            string id,
            [FromForm] UserPasswordUpdateModel model)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Update(idParsed, model);

            return ResponseHelper.Success(_service.GetOne(idParsed));
        }

        /// <summary>
        /// Удаление пользователя (для админа)
        /// </summary>
        /// <param name="id">ИД пользователя</param>
        [HttpDelete("{id}"), HasRightAuthorize(RightRequirement.CombinationRule.Or, Rights.UserDelete, Rights.UserInteractAll)]
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

        /// <summary>
        /// Самостоятельное обновление данных пользователя
        /// </summary>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult UpdateSelf([FromForm] UserSelfUpdateModel model)
        {
            if (!User.TryGetUserId(out var userId))
            {
                return ResponseHelper.Error(ApiStrings.Common_Error_Unauthorized, HttpStatusCode.Unauthorized);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Update(userId, model);

            return ResponseHelper.Success(_service.GetOne(userId));
        }

        /// <summary>
        /// Самостоятельная смена пароля пользователя
        /// </summary>
        /// <param name="model">Модель данных</param>
        /// <returns></returns>
        [HttpPut("password")]
        public IActionResult UpdatePasswordSelf([FromForm] UserPasswordUpdateModel model)
        {
            if (!User.TryGetUserId(out var userId))
            {
                return ResponseHelper.Error(ApiStrings.Common_Error_Unauthorized, HttpStatusCode.Unauthorized);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Update(userId, model);

            return ResponseHelper.Success(_service.GetOne(userId));
        }

        /// <summary>
        /// Самостоятельная загрузка аватара пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("avatar")]
        public IActionResult UploadAvatarSelf([FromForm] UserUpdateAvatarModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var url = _service.UploadAvatar(model);

            if (string.IsNullOrEmpty(url))
            {
                return ResponseHelper.Error(ApiStrings.User_Error_AvatarUpload);
            }
            else
            {
                return ResponseHelper.Success(url);
            }
        }

        /// <summary>
        /// Самомостоятельное удаление аватара пользователя
        /// </summary>
        /// <returns></returns>
        [HttpDelete("avatar")]
        public IActionResult DeleteAvatarSelf()
        {
            _service.DeleteAvatar();

            return ResponseHelper.Success();
        }
    }
}
