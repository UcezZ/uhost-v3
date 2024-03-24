using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Uhost.Core.Data;
using Uhost.Core.Models;

namespace Uhost.Core.Extensions
{
    // Помощь в сортировке запросов
    public static class SortingExtensions
    {
        // Проверим, что параметр соответвует сортировке в прямом порядке
        private static bool IsAsc(string param)
        {
            return string.IsNullOrEmpty(param) || param.Trim().ToLower() == "asc";
        }

        // Добавляем в строку ключевое слово направление сортировки
        public static string SetDirection(string direct, string query = null)
        {
            if (!string.IsNullOrEmpty(query))
            {
                return query
                    .Split(',')
                    .Select(e => $"{e.Trim()} {(IsAsc(direct) ? "ASC" : "DESC")}")
                    .Join(", ");
            }

            return string.Empty;
        }

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, BaseQueryModel query) where TEntity : BaseEntity =>
            source.OrderBy(SetDirection(query.SortDirection, query.SortBy));

        // Сортируем набор Entity по переданной строке
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByStrValues) where TEntity : BaseEntity
        {
            if (string.IsNullOrEmpty(orderByStrValues))
            {
                return source;
            }

            var queryExpr = source.Expression;
            var methodAsc = "OrderBy";
            var methodDesc = "OrderByDescending";

            var orderByValues = orderByStrValues.Trim().Split(',').Select(x => x.Trim()).ToList();

            foreach (var orderPairCommand in orderByValues)
            {
                var command = orderPairCommand.ToUpper().EndsWith("DESC") ? methodDesc : methodAsc;

                //Get propertyname and remove optional ASC or DESC
                var propertyName = orderPairCommand.Split(' ')[0].Trim();

                var type = typeof(TEntity);
                var parameter = Expression.Parameter(type, "p");

                PropertyInfo property;
                MemberExpression propertyAccess;

                if (propertyName.Contains('.'))
                {
                    // support to be sorted on child fields. 
                    var childProperties = propertyName.Split('.');

                    property = type.GetPropertyIgnoreCase(childProperties[0]);

                    if (property == null)
                    {
                        continue;
                    }

                    propertyAccess = Expression.MakeMemberAccess(parameter, property);

                    for (int i = 1; i < childProperties.Length; i++)
                    {
                        property = property.PropertyType.GetPropertyIgnoreCase(childProperties[i]);

                        if (property == null)
                        {
                            continue;
                        }

                        propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                    }
                }
                else
                {
                    property = type.GetPropertyIgnoreCase(propertyName);

                    if (property == null)
                    {
                        continue;
                    }

                    propertyAccess = Expression.MakeMemberAccess(parameter, property);
                }

                var orderByExpression = Expression.Lambda(propertyAccess, parameter);

                queryExpr = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType }, queryExpr, Expression.Quote(orderByExpression));

                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }

            return source.Provider.CreateQuery<TEntity>(queryExpr);
        }
    }
}
