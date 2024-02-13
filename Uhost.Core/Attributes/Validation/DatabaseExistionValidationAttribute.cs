using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация существования сущности по определённому полю. Поддерживает перечисления.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DatabaseExistionValidationAttribute : ValidationAttribute
    {
        private readonly string _tableName;
        private readonly string _propertyName;
        private readonly string _sqlPredicate;
        private readonly Type _propertyType;
        private readonly bool _includeDeleted, _nullable, _hasDeletedAtField;

        /// <summary>
        /// <inheritdoc cref="DatabaseExistionValidationAttribute"/>
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <param name="propertyName">Имя поля</param>
        /// <param name="includeDeleted">Учитывать удалённые</param>
        /// <param name="nullable">Может быть null</param>
        /// <param name="sqlPredicate">Дополнительные условия SQL WHERE</param>
        /// <exception cref="ArgumentException"></exception>
        public DatabaseExistionValidationAttribute(Type entityType, string propertyName, bool includeDeleted = false, bool nullable = false, string sqlPredicate = null)
        {
            _tableName = Tools.GetEntityTableName(entityType) ?? throw new ArgumentException($"Invalid type specified ${entityType?.Name ?? "null"}", nameof(entityType));
            var property = entityType.GetProperty(propertyName) ?? throw new ArgumentException($"Invalid type specified ${propertyName}", nameof(propertyName));
            _hasDeletedAtField = entityType.GetProperty("DeletedAt") != null;
            _propertyName = propertyName;
            _propertyType = property.PropertyType;
            _includeDeleted = includeDeleted;
            _nullable = nullable;
            _sqlPredicate = sqlPredicate;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var isNullOrEmpty =
                value == null
                || value.ToString() == string.Empty
                || int.TryParse(value?.ToString() ?? string.Empty, out var intValue) && intValue == 0
                || value is IEnumerable && !((IEnumerable)value).Cast<object>().Any() && value is not string;

            // если значение пустое и мы разрешили нулабельность - выходим
            if (isNullOrEmpty && _nullable)
            {
                return ValidationResult.Success;
            }

            // конвертируем содержимое значения в тип поля сущности, если типы не совпали
            var converter = TypeDescriptor.GetConverter(_propertyType);

            if (value.GetType() != _propertyType && !(value is IEnumerable && value is not string))
            {
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

            // проверяем существование сущности
            using (var ctx = validationContext.GetDbContextScope<PostgreSqlDbContext>())
            using (var cmd = ctx.Database.GetDbConnection().CreateCommand())
            {
                var wheres = new List<string>();

                // условие фильтрации - = или IN
                if (value is not string && value is IEnumerable nonGenericEnumerable)
                {
                    wheres.Add($"\"{_propertyName}\" IN ({nonGenericEnumerable.Cast<object>().Select(e => $"'{e}'").Distinct().Join(", ")})");
                }
                else
                {
                    wheres.Add($"\"{_propertyName}\" = @val");
                    cmd.Parameters.Add(new NpgsqlParameter("val", value));
                }

                // влючая удалённые
                if (!_includeDeleted && _hasDeletedAtField)
                {
                    wheres.Add("\"DeletedAt\" IS NULL");
                }

                // дополнительный SQL
                if (!string.IsNullOrWhiteSpace(_sqlPredicate))
                {
                    wheres.Add($"({_sqlPredicate})");
                }

                cmd.CommandText = $"SELECT DISTINCT \"{_propertyName}\" FROM \"{_tableName}\" WHERE {wheres.Join(" AND ")}";

                if (cmd.Connection.State != ConnectionState.Open)
                {
                    cmd.Connection.Open();
                }

                using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (value is IEnumerable && value is not string)
                    {
                        var target = (value as IEnumerable)
                            .Cast<object>()
                            .Select(e => e.ToString())
                            .Distinct()
                            .ToList();
                        var fetched = new List<string>();

                        while (reader.Read())
                        {
                            fetched.Add(reader[0] == DBNull.Value ? null : reader[0].ToString());
                        }

                        // проверяем что в базе есть всё
                        if (fetched.Count < target.Count)
                        {
                            // выбираем что не нашли
                            var missed = target
                                .Except(fetched)
                                .ToList();

                            return new ValidationResult(ErrorMessageString?.Format(missed.Join(", ")));
                        }

                        return ValidationResult.Success;
                    }
                    else
                    {
                        return reader.Read() ? ValidationResult.Success : new ValidationResult(ErrorMessageString?.Format(value));
                    }
                }
            }
        }
    }
}
