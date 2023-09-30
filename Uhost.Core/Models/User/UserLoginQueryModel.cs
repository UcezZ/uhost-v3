using Uhost.Core.Attributes.Validation;

namespace Uhost.Core.Models.User
{
    public class UserLoginQueryModel
    {
        /// <summary>
        /// Логин или почта
        /// </summary>
        [StringLengthValidation(minLength: 3, maxLength: 50, allowEmpty: false)]
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [StringLengthValidation(minLength: 3, maxLength: 50, trim: false, allowEmpty: false)]
        public string Password { get; set; }
    }
}
