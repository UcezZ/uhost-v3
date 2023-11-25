using System;
using System.ComponentModel.DataAnnotations;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация строк с возможностью обрезания пробелов. 
    /// <br/>
    /// <b>Не использует</b> <see cref="ValidationAttribute.ErrorMessageResourceName"/> и <see cref="ValidationAttribute.ErrorMessageResourceType"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class StringLengthValidationAttribute : ValidationAttribute
    {
        private readonly int _min, _max;
        private readonly bool _trim, _empty;

        /// <summary>
        /// <inheritdoc cref="StringLengthValidationAttribute"/>
        /// </summary>
        /// <param name="minLength">Минимальная длина</param>
        /// <param name="maxLength">Максимальная длина</param>
        /// <param name="trim">Обрезать пробелы</param>
        /// <param name="allowEmpty">Разрешить пустую строку</param>
        public StringLengthValidationAttribute(int minLength = 0, int maxLength = int.MaxValue, bool trim = true, bool allowEmpty = true)
        {
            _min = minLength;
            _max = maxLength;
            _trim = trim;
            _empty = allowEmpty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var strValue = value is string str ? str : value?.ToString() ?? string.Empty;

            if (_trim)
            {
                strValue = strValue.Trim();
            }

            if (!_empty && string.IsNullOrEmpty(strValue))
            {
                return new ValidationResult(CoreStrings.Common_Error_RequiredFmt.Format(validationContext.MemberName));
            }

            if (!(_empty && string.IsNullOrEmpty(strValue)))
            {
                if (strValue.Length < _min)
                {
                    return new ValidationResult(CoreStrings.Common_Error_StringTooShortFmt.Format(validationContext.MemberName, _min));
                }
                if (strValue.Length > _max)
                {
                    return new ValidationResult(CoreStrings.Common_Error_StringTooLongFmt.Format(validationContext.MemberName, _max));
                }
            }

            if (_trim)
            {
                validationContext.ObjectType?
                    .GetProperty(validationContext.MemberName)?
                    .SetValue(validationContext.ObjectInstance, strValue);
            }

            return ValidationResult.Success;
        }
    }
}
