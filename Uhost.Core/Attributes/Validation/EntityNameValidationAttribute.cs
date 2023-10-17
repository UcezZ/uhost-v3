using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Properties;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация имени сущности
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class EntityNameValidationAttribute : ValidationAttribute
    {
        private readonly IEnumerable<Type> _entities;
        private readonly bool _allowEmpty;

        /// <summary>
        /// Валидация имени сущности
        /// </summary>
        /// <param name="allowEmpty">Определяет, можно ли не указывать значение</param>
        public EntityNameValidationAttribute(bool allowEmpty = false)
        {
            _entities = Tools.EnumerateEntities();
            _allowEmpty = allowEmpty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var name = value?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) && _allowEmpty)
            {
                return ValidationResult.Success;
            }

            return _entities.Any(e => e.Name.Equals(name, StringComparison.InvariantCulture))
                ? ValidationResult.Success
                : new ValidationResult(CoreStrings.Common_Error_EntityNotFound);
        }
    }
}
