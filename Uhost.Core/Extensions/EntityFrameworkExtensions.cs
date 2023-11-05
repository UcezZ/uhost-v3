using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq;
using System.Linq.Expressions;
using Uhost.Core.Common;

namespace Uhost.Core.Extensions
{
    public static class EntityFrameworkExtensions
    {
        /// <summary>
        /// Определяет в контексте функции из <see cref="PostgreSqlFunctions"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder DefineDbFunctions(this ModelBuilder builder)
        {
            builder
                .HasDbFunction(typeof(PostgreSqlFunctions).GetMethod(nameof(PostgreSqlFunctions.TrgmWordSimilarity)))
                .HasTranslation(e => new SqlFunctionExpression("word_similarity", e, false, e.Select(b => false), typeof(double), null));

            builder
                .HasDbFunction(typeof(PostgreSqlFunctions).GetMethod(nameof(PostgreSqlFunctions.TrgmAreSimilar)))
                .HasTranslation(e => new SqlBinaryExpression(ExpressionType.Modulo, e.First(), e.Last(), typeof(bool), null));

            return builder;
        }
    }
}
