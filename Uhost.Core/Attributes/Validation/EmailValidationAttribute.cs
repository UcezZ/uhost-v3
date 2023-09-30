using System.ComponentModel.DataAnnotations;
using Uhost.Core.Extensions;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация e-mail
    /// </summary>
    public class EmailValidationAttribute : ValidationAttribute
    {
        private readonly bool _allowNull;

        /// <summary>
        /// <inheritdoc cref="EmailValidationAttribute"/>
        /// </summary>
        /// <param name="allowNull"></param>
        public EmailValidationAttribute(bool allowNull = false)
        {
            _allowNull = allowNull;
        }

        public override bool IsValid(object value)
        {
            if (_allowNull && string.IsNullOrEmpty(value?.ToString()))
            {
                return true;
            }

            return value != null && value.ToString().IsValidEmailAddress();
        }
    }
}
