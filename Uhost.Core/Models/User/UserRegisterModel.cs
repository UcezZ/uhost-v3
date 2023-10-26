using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.User;

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
        [StringLengthValidation(maxLength: 64)]
        public string Password { get; set; }

        /// <summary>
        /// Повтор пароля
        /// </summary>
        [StringLengthValidation(maxLength: 64), FieldEqualsValidation(nameof(Password))]
        public string PasswordConfirm { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.Password = (Password + CoreSettings.PasswordSalt).ComputeHash(HasherExtensions.EncryptionMethod.SHA256);

            return entity;
        }
    }
}
