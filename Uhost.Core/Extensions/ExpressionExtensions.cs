using System;
using System.Linq;
using System.Linq.Expressions;

namespace Uhost.Core.Extensions
{
    public static class ExpressionExtensions
    {
        public static object GetValue(this Expression expression)
        {
            var memberExpression = expression as MemberExpression;

            if (memberExpression != null)
            {
                var objectMember = Expression.Convert(memberExpression, typeof(object));
                var getterLambda = Expression.Lambda<Func<object>>(objectMember);
                var getter = getterLambda.Compile();

                return getter();
            }

            var innerExpressionProp = expression
                .GetType()
                .GetProperties()
                .FirstOrDefault(e => e.Name.Equals("expression", StringComparison.InvariantCultureIgnoreCase));

            var constExpression = expression as ConstantExpression;

            if (constExpression == null && innerExpressionProp != null)
            {
                var innerExpression = innerExpressionProp.GetValue(expression) as Expression;

                return innerExpression?.GetValue();
            }

            return constExpression.Value;
        }
    }
}
