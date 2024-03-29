﻿using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserPasswordUpdateModel : IEntityFillable<Entity>
    {
        /// <summary>
        /// Новый пароль
        /// </summary>
        [StringLengthValidation(minLength: 6, maxLength: 64, allowEmpty: false, trim: false)]
        public string Password { get; set; }

        /// <summary>
        /// Подтверждение пароля
        /// </summary>
        [StringLengthValidation(minLength: 6, maxLength: 64, allowEmpty: false, trim: false), FieldEqualsValidation(nameof(Password))]
        public string PasswordConfirm { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Password = (Password + CoreSettings.PasswordSalt).ComputeHash(HasherExtensions.EncryptionMethod.SHA256);

            return entity;
        }
    }
}
