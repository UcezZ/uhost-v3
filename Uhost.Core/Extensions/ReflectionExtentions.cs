using System;
using System.Linq;
using System.Reflection;

namespace Uhost.Core.Extensions
{
    public static class ReflectionExtentions
    {
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetProperties().FirstOrDefault(
                item => string.Equals(
                    item.Name,
                    name,
                    StringComparison.CurrentCultureIgnoreCase
                )
            );
        }
    }
}
