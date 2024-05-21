using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.User;
using UserRoleEntity = Uhost.Core.Data.Entities.UserRole;

namespace Uhost.Core.Models.User
{
    public class UserRegisterModel : UserBaseModel
    {
        /// <summary>
        /// Email
        /// </summary>
        [EmailValidation]
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [StringLengthValidation(minLength: 6, maxLength: 64, allowEmpty: false, trim: false)]
        public string Password { get; set; }

        /// <summary>
        /// Повтор пароля
        /// </summary>
        [StringLengthValidation(minLength: 6, maxLength: 64, allowEmpty: false, trim: false), FieldEqualsValidation(nameof(Password))]
        public string PasswordConfirm { get; set; }

        /// <summary>
        /// ИД ролей пользователя. Для внутреннего использованя
        /// </summary>
        internal IEnumerable<int> RoleIds { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.Password = (Password + CoreSettings.PasswordSalt).ComputeHash(HasherExtensions.EncryptionMethod.SHA256);
            entity.Email = Email;

            if (RoleIds != null)
            {
                entity.UserRoles = RoleIds
                    .Select(e => new UserRoleEntity
                    {
                        RoleId = e
                    })
                    .ToList();
            }

            return entity;
        }
    }
}
