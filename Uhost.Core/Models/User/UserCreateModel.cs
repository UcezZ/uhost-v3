using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.User;
using RoleEntity = Uhost.Core.Data.Entities.Role;
using UserRoleEntity = Uhost.Core.Data.Entities.UserRole;

namespace Uhost.Core.Models.User
{
    public class UserCreateModel : UserBaseModel
    {
        /// <summary>
        /// Коллекция ИД ролей
        /// </summary>
        [DatabaseExistionValidation(typeof(RoleEntity), nameof(RoleEntity.Id), nullable: true, ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Role_Error_NotFoundByIds))]
        public IEnumerable<int> RoleIds { get; set; }

        /// <summary>
        /// E-Mail
        /// </summary>
        [EmailValidation(true)]
        public string Email { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            if (RoleIds != null)
            {
                entity.UserRoles ??= new List<UserRoleEntity>();
                entity.UserRoles.Clear();
                entity.UserRoles.AddRange(RoleIds.Distinct().Select(e => new UserRoleEntity { RoleId = e }).ToList());
            }

            if (!string.IsNullOrEmpty(Email))
            {
                entity.Email = Email;
            }

            return entity;
        }
    }
}
