using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация enum'ов в строке. Применимо к массиву и перечислению (<see cref="IEnumerable{T}"/>) строк.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class EnumValidationAttribute : ValidationAttribute
    {
        private readonly Type _type;
        private readonly List<string> _values;
        private readonly object _ifNullOrEmpty;
        private readonly bool _allowEmpty;

        /// <summary>
        /// <inheritdoc cref="EnumValidationAttribute"/>
        /// </summary>
        /// <param name="type">typeof(enum)</param>
        /// <param name="ifNullOrEmpty">Значение, которое будет записано в свойство, если передано null</param>
        /// <param name="allowEmpty">Разрешить значение null. Пропускает все остальные проверки, не записывает ifNullOrEmpty в свойство</param>
        /// <param name="blacklist">Исключаемые значения, использовать nameof</param>
        /// <param name="whitelist">Белый список значений, использовать nameof</param>
        /// <exception cref="ArgumentException"></exception>
        public EnumValidationAttribute(Type type, object ifNullOrEmpty = null, bool allowEmpty = false, string[] blacklist = null, string[] whitelist = null)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException("Should be a type of enum", nameof(type));
            }

            _type = type;
            _ifNullOrEmpty = ifNullOrEmpty;
            _allowEmpty = allowEmpty;
            _values ??= new List<string>();

            foreach (var value in _type.GetEnumValues())
            {
                _values.Add(value.ToString());
            }

            if (blacklist != null)
            {
                if (blacklist.Any(e => !_values.Contains(e)))
                {
                    throw new ArgumentException($"All items should be inner type of enum '{type.FullName}'", nameof(whitelist));
                }
                else
                {
                    _values.RemoveAll(e => blacklist.Contains(e));
                }
            }

            if (whitelist != null)
            {
                if (whitelist.Any(e => !_values.Contains(e)))
                {
                    throw new ArgumentException($"All items should be inner type of enum '{type.FullName}'", nameof(whitelist));
                }

                _values.RemoveAll(e => !whitelist.Contains(e));
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((value == null || string.IsNullOrEmpty(value?.ToString() ?? string.Empty)) && _allowEmpty)
            {
                return ValidationResult.Success;
            }

            var errorMessage = string.Format(ErrorMessageString, string.Join(", ", _values));

            if ((value == null || value is string str && string.IsNullOrEmpty(str)) && _ifNullOrEmpty != null)
            {
                var prop = validationContext.ObjectType.GetProperty(validationContext.MemberName);
                str = _ifNullOrEmpty.ToString();

                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(validationContext.ObjectInstance, str);
                }
            }
            else
            {
                if (value == null)
                {
                    return new ValidationResult(errorMessage);
                }
                else
                {
                    str = value.ToString();
                }
            }

            if (value is string[] array && array.All(_values.Contains))
            {
                return ValidationResult.Success;
            }

            if (value is IEnumerable<string> enumerable && enumerable.All(_values.Contains))
            {
                return ValidationResult.Success;
            }

            return _values.Contains(str)
                ? ValidationResult.Success
                : new ValidationResult(errorMessage);
        }
    }
}
