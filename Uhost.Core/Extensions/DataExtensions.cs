using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Uhost.Core.Models;

namespace Uhost.Core.Extensions
{
    /// <summary>
    /// <see cref="DataRow"/> extensions
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// Get value of type <typeparamref name="T"/> from column <paramref name="columnName"/> of <paramref name="dataRow"/>
        /// </summary>
        /// <inheritdoc cref="TryGetValue{T}(DataRow, string, out T, out Exception)"/>
        public static bool TryGetValue<T>(this DataRow dataRow, string columnName, out T value) =>
            dataRow.TryGetValue(columnName, out value, out _);

        /// <summary>
        /// Get value of type <typeparamref name="T"/> from column <paramref name="columnName"/> of <paramref name="dataRow"/>
        /// </summary>
        /// <typeparam name="T">Tagret type</typeparam>
        /// <param name="dataRow">DataRow instance</param>
        /// <param name="columnName">Column name</param>
        /// <param name="value">Output value</param>
        /// <param name="exception">Output exception</param>
        /// <returns><c>true</c> if cast/convert successful, <c>false</c> either</returns>
        public static bool TryGetValue<T>(this DataRow dataRow, string columnName, out T value, out Exception exception)
        {
            object rawValue;
            try
            {
                rawValue = dataRow[columnName];
            }
            catch (Exception e)
            {
                value = default;
                exception = e;
                return false;
            }

            if (rawValue is null || rawValue is DBNull)
            {
                value = default;
                exception = new Exception($"{nameof(dataRow)}[{columnName}]");
                return false;
            }

            if (rawValue.GetType() == typeof(T))
            {
                value = (T)rawValue;
                exception = default;
                return true;
            }

            try
            {
                value = (T)rawValue;
                exception = default;
                return true;
            }
            catch { }

            if (typeof(T) == typeof(bool))
            {
                try
                {
                    value = (T)(object)Convert.ToBoolean(rawValue);
                    exception = default;
                    return true;
                }
                catch { }
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));

            if (converter == null)
            {
                value = default;
                exception = new Exception(nameof(converter));
                return false;
            }

            try
            {
                value = (T)converter.ConvertFromString(rawValue.ToString());
                exception = default;
                return true;
            }
            catch (Exception e)
            {
                value = default;
                exception = e;
                return false;
            }
        }

        /// <summary>
        /// Преобразует <see cref="DataTable"/> в коллекцию моделей <typeparamref name="TModel"/>.
        /// </summary>
        public static ICollection<TModel> ToModelCollection<TModel>(this DataTable dataTable) where TModel : IDataRowLoadable, new()
        {
            var models = new List<TModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                var model = new TModel();
                model.LoadFromDataRow(row);
                models.Add(model);
            }

            return models;
        }
    }
}
