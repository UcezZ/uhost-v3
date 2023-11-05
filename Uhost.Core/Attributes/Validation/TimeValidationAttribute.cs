using System;
using System.ComponentModel.DataAnnotations;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация строки времени
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TimeValidationAttribute : ValidationAttribute
    {
        private readonly TimeSpan? _max;
        private readonly TimeSpan? _min;
        private readonly bool _allowEmpty;

        public TimeValidationAttribute(int minSeconds = 0, int maxSeconds = 0, bool allowEmpty = true)
        {
            _min = minSeconds == 0 ? null : TimeSpan.FromSeconds((int)minSeconds);
            _max = maxSeconds == 0 ? null : TimeSpan.FromSeconds((int)maxSeconds);
            _allowEmpty = allowEmpty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(value?.ToString()) && _allowEmpty)
            {
                return ValidationResult.Success;
            }

            if (!TimeSpan.TryParse(value?.ToString(), out var parsed))
            {
                return new ValidationResult(CoreStrings.Common_Error_TimeSpanInvalidFmt.Format(value));
            }

            if (_min != null && parsed < _min)
            {
                return new ValidationResult(CoreStrings.Common_Error_TimeSpanTooShortFmt.Format(_min));
            }

            if (_max != null && parsed > _max)
            {
                return new ValidationResult(CoreStrings.Common_Error_TimeSpanTooLongFmt.Format(_max));
            }

            return ValidationResult.Success;
        }
    }
}
