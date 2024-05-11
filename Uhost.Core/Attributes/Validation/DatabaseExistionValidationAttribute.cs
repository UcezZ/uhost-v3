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
            if (string.IsNullOrEmpty(_tableName = Tools.GetEntityTableName(entityType)))
            {
                throw new ArgumentException($"Invalid type specified ${entityType?.Name ?? "null"}", nameof(entityType));
            }

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
            var enumerable = (value is string ? null : value as IEnumerable)?.Cast<object>();
            var isEnumerable = enumerable != null;

            // пришла пустота
            var isEmpty = string.IsNullOrEmpty(value?.ToString())

                // если пришёл целый ноль и поле сущности - целое
                || _propertyType.IsAssignableTo(typeof(int)) && int.TryParse(value?.ToString(), out var intValue) && intValue == default

                // если пришёл дробный ноль и поле сущности - дробное
                || _propertyType.IsAssignableTo(typeof(double)) && double.TryParse(value?.ToString(), out var doubleValue) && doubleValue == default

                // если пришло пустое перечисление
                || isEnumerable && !enumerable.Any();

            // если значение пустое и мы разрешили нулабельность - выходим
            if (isEmpty && _nullable)
            {
                return ValidationResult.Success;
            }

            // если пришла пустота и не разрешили нулабельность - выходим с ошибкой
            if (isEmpty && !_nullable)
            {
                return new ValidationResult(CoreStrings.Common_Error_RequiredFmt.Format(validationContext?.MemberName ?? _propertyName));
            }

            // если поле это Id и пришло не то - выходим с ошибкой
            if (_propertyName == nameof(BaseEntity.Id) && !isEnumerable && !(value?.ToString() ?? string.Empty).TryParsePositiveInt(out _))
            {
                return new ValidationResult(CoreStrings.Common_Error_Invalid);
            }

            // конвертируем содержимое значения в тип поля сущности, если типы не совпали
            var converter = TypeDescriptor.GetConverter(_propertyType);

            if (value.GetType() != _propertyType && !isEnumerable)
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
            using (var ctx = validationContext.GetDbContextInstance<PostgreSqlDbContext>())
            using (var cmd = ctx.Database.GetDbConnection().CreateCommand())
            {
                var wheres = new List<string>();

                // условие фильтрации - = или IN
                if (isEnumerable)
                {
                    var values = enumerable
                        .Cast<object>()
                        .Select(e => e.TryConvertTo(_propertyType, out var converted) ? converted : null)
                        .Where(e => e != null)
                        .Distinct()
                        .ToList();

                    if (!values.Any())
                    {
                        return new ValidationResult(CoreStrings.Common_Error_Invalid);
                    }

                    var typedValues = values.ToTypedList(_propertyType);

                    wheres.Add($"\"{_propertyName}\" = ANY(@val)");
                    cmd.Parameters.Add(new NpgsqlParameter("val", typedValues));
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
                    if (isEnumerable)
                    {
                        var target = enumerable
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
