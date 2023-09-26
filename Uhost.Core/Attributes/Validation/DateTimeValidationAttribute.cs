using System;
using System.ComponentModel.DataAnnotations;
using Uhost.Core.Extensions;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация даты-времени по форматам, совместимым с <see cref="DateTimeExtensions"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    internal class DateTimeValidationAttribute : ValidationAttribute
    {
        private readonly bool _allowNull;

        /// <summary>
        /// <inheritdoc cref="DateTimeValidationAttribute"/>.
        /// </summary>
        /// <param name="allowNull">Разрешить пустые значения</param>
        public DateTimeValidationAttribute(bool allowNull = false)
        {
            _allowNull = allowNull;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.ToString() == string.Empty)
            {
                if (_allowNull)
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMessageString);
            }

            if (value.ToString().TryParseDateTime(out _))
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(ErrorMessageString);
            }
        }
    }
}
