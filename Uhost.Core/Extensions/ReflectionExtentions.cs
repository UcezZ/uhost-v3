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

        /// <summary>
        /// Создаёт экземпляр объекта типа <paramref name="type"/>, вызывая конструктор с параметрами <paramref name="args"/>
        /// </summary>
        /// <param name="type">Тип объекта</param>
        /// <param name="args">Параметры конструктора</param>
        /// <returns></returns>
        public static object Instantiate(this Type type, params object[] args)
        {
            var types = args
                .Select(e => e.GetType())
                .ToArray();
            var constructor = type.GetConstructor(types);

            return constructor.Invoke(args);
        }
    }
}
