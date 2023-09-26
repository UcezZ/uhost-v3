using Uhost.Core.Attributes.Validation;
using Uhost.Core.Common;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserRegisterModel : UserBaseModel
    {
        [StringLengthValidation(maxLength: 64)]
        public string Password { get; set; }

        [StringLengthValidation(maxLength: 64), FieldEqualsValidation(nameof(Password))]
        public string PasswordConfirm { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.Password = Hasher.ComputeHash(Password + CoreSettings.PasswordSalt, Hasher.EncryptionMethod.SHA256);

            return entity;
        }
    }
}
