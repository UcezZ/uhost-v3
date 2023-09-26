using System;
using System.ComponentModel.DataAnnotations;

namespace Uhost.Core.Attributes.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FieldEqualsValidationAttribute : ValidationAttribute
    {
        private readonly string _propertyName;

        /// <summary>
        /// Валидация заполнения при условии равенства другого свойства модели
        /// </summary>
        /// <param name="propertyName">nameof(Property)</param>
        /// <exception cref="ArgumentException"></exception>
        public FieldEqualsValidationAttribute(string propertyName)
        {
            _propertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var prop = validationContext.ObjectType.GetProperty(_propertyName);

            if (prop == null)
            {
                throw new ArgumentException($"Property named '{_propertyName}' is not defined in model '{validationContext.DisplayName}'");
            }
            if (value.GetType() != prop.PropertyType)
            {
                throw new ArgumentException($"Given property value type ({value.GetType().Name}) doesn't match model's property value type ({prop.PropertyType.Name})");
            }
            if (prop.GetValue(validationContext.ObjectInstance)?.GetHashCode() != value?.GetHashCode() || value == null || value.ToString() == string.Empty)
            {
                return new ValidationResult(string.Format(ErrorMessageString, validationContext.MemberName));
            }

            return ValidationResult.Success;
        }
    }
}
