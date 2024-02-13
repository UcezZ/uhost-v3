using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
            var constructor = type.GetConstructor(types) ?? throw new InvalidOperationException($"The type '{type?.FullName}' has no constructor with args: [{args?.Select(e => $"{e?.GetType().Name}: {e}").Join(", ")}]");

            return constructor.Invoke(args);
        }

        /// <summary>
        /// Определяет, является ли метод асинхронным
        /// </summary>
        /// <param name="methodInfo">Метод</param>
        /// <returns></returns>
        public static bool IsAsync(this MethodInfo methodInfo)
        {
            return typeof(Task).IsAssignableFrom(methodInfo.ReturnType) ||
                  (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
        }

        ///<inheritdoc cref="Type.IsAssignableTo(Type?)"/>
        public static bool IsAssignableTo<T>(this Type type) => type.IsAssignableTo(typeof(T));

        ///<inheritdoc cref="Type.IsAssignableFrom(Type?)"/>
        public static bool IsAssignableFrom<T>(this Type type) => type.IsAssignableFrom(typeof(T));
    }
}
