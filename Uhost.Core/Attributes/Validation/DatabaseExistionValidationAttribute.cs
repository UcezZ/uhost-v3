using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Properties;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация существования сущности по определённому полю
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DatabaseExistionValidationAttribute : ValidationAttribute
    {
        private readonly string _tableName;
        private readonly string _propertyName;
        private readonly Type _propertyType;
        private readonly bool _includeDeleted, _nullable;

        /// <summary>
        /// Валидация существования сущности по определённому полю
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="propertyName">Имя поля</param>
        /// <param name="includeDeleted">Учитывать удалённые</param>
        /// <param name="nullable">Может быть null</param>
        /// <exception cref="ArgumentException"></exception>
        public DatabaseExistionValidationAttribute(Type entityType, string propertyName, bool includeDeleted = false, bool nullable = false)
        {
            if (string.IsNullOrEmpty(_tableName = Tools.GetEntityTableNameByEntityType(entityType)))
            {
                throw new ArgumentException($"Invalid type specified ${entityType?.Name ?? "null"}", nameof(entityType));
            }

            var property = entityType.GetProperty(propertyName) ?? throw new ArgumentException($"Invalid type specified ${propertyName}", nameof(propertyName));
            _propertyName = propertyName;
            _propertyType = property.PropertyType;
            _includeDeleted = includeDeleted;
            _nullable = nullable;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if ((value == null || value.ToString() == string.Empty || int.TryParse(value?.ToString() ?? string.Empty, out var intValue) && intValue == 0) && _nullable)
            {
                return ValidationResult.Success;
            }

            if (value.GetType() != _propertyType)
            {
                var converter = TypeDescriptor.GetConverter(_propertyType);

                if (converter == null)
                {
                    return new ValidationResult(CoreStrings.Common_Error_Invalid);
                }
                try
                {
                    value = converter.ConvertFromString(value.ToString());
                }
                catch
                {
                    return new ValidationResult(CoreStrings.Common_Error_Invalid);
                }
            }

            using (var cmd = validationContext.GetService<PostgreSqlDbContext>().Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = $"SELECT * FROM \"{_tableName}\" WHERE \"{_propertyName}\" = @val {(_includeDeleted ? "" : "AND \"DeletedAt\" IS NULL ")}LIMIT 1";
                cmd.Parameters.Add(new NpgsqlParameter("val", value));

                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    return reader.Read() ? ValidationResult.Success : new ValidationResult(string.Format(ErrorMessageString, value));
                }
            }
        }
    }
}
