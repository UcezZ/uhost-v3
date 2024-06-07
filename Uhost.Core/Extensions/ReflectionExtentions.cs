using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Uhost.Core.Extensions
{
    public static class ReflectionExtentions
    {
        public static PropertyInfo GetPropertyIgnoreCase(this Type type, string name)
        {
            return type.GetProperties()
                .FirstOrDefault(e => e.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Создаёт экземпляр объекта типа <paramref name="type"/>, вызывая конструктор с параметрами <paramref name="args"/>
        /// </summary>
        /// <param name="type">Тип объекта</param>
        /// <param name="args">Параметры конструктора</param>
        /// <returns></returns>
        public static object Instantiate(this Type type, params object[] args)
        {
            var argTypes = args
                .Select(e => e.GetType())
                .ToArray();

            var constructor = type.GetConstructor(argTypes);

            if (constructor == null)
            {
                if (type.IsInterface)
                {
                    var implementingClassType = FindImplementingClass(type, argTypes) ?? throw new InvalidOperationException($"Interface '{type.Name}' has no implementation with constructor having args [{argTypes.Select(e => e.FullName).Join(", ")}]");
                    constructor = implementingClassType.GetConstructor(argTypes);
                }
                else
                {
                    throw new InvalidOperationException($"The type '{type?.Name}' has no constructor with args: [{args?.Select(e => $"{e?.GetType().Name}: {e}").Join(", ")}]");
                }
            }

            var instance = constructor.Invoke(args);

            return instance;
        }

        /// <summary>
        /// Поиск класса, реализуюшего интерфейс типа <paramref name="interfaceType"/>, и имеющего аргументы конструктора <paramref name="argTypes"/>.
        /// </summary>
        /// <param name="interfaceType">Тип интерфейса</param>
        /// <param name="argTypes">Типы параметров конструктора класса</param>
        /// <returns></returns>
        public static Type FindImplementingClass(this Type interfaceType, Type[] argTypes)
        {
            if (!interfaceType.IsInterface)
            {
                return interfaceType;
            }

            var assemblies = AssemblyLoadContext.GetLoadContext(Assembly.GetCallingAssembly()).Assemblies;

            return assemblies
                .Where(e => e != null)
                .SelectMany(e => e.GetTypes())
                .Where(e => e != null && !e.IsInterface && !e.IsAbstract && e.IsAssignableTo(interfaceType))
                .FirstOrDefault(e => e.GetConstructor(argTypes) != null);
        }

        /// <summary>
        /// Определяет, является ли метод асинхронным
        /// </summary>
        /// <param name="methodInfo">Метод</param>
        /// <returns></returns>
        public static bool IsAsync(this MethodInfo methodInfo)
        {
            return methodInfo.ReturnType.IsAssignableTo<Task>() ||
                  (methodInfo.ReturnType.IsGenericType && methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
        }

        ///<inheritdoc cref="Type.IsAssignableTo(Type?)"/>
        public static bool IsAssignableTo<T>(this Type type) => type.IsAssignableTo(typeof(T));

        ///<inheritdoc cref="Type.IsAssignableFrom(Type?)"/>
        public static bool IsAssignableFrom<T>(this Type type) => type != null && type.IsAssignableFrom(typeof(T));


        /// <summary>
        /// Создаёт атрибут из данных атрибута
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="customAttributeData"></param>
        /// <returns></returns>
        public static TAttribute ToAttributeInstance<TAttribute>(this CustomAttributeData customAttributeData) where TAttribute : Attribute
        {
            var args = customAttributeData.ConstructorArguments
                .Select(e => e.Value)
                .ToArray();

            var instance = customAttributeData.Constructor.Invoke(args);

            if (instance is TAttribute targetAttribute)
            {
                foreach (var arg in customAttributeData.NamedArguments)
                {
                    typeof(TAttribute)
                        .GetProperty(arg.MemberName)?
                        .SetValue(targetAttribute, arg.TypedValue.Value);
                }

                return targetAttribute;
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<T> AsTypedEnumerable<T, S>(this IEnumerable<S> enumerable)
        {
            return enumerable.Select(e => e.TryConvertTo<T>(out var converted) ? converted : default);
        }

        public static object ToTypedList<T>(this IEnumerable<T> enumerable, Type target)
        {
            var typedEnumerableMethod = typeof(ReflectionExtentions)
                .GetMethod(nameof(AsTypedEnumerable))
                .MakeGenericMethod(target, typeof(T));
            var typedEnumerable = typedEnumerableMethod.Invoke(null, new object[] { enumerable });

            var toListMethod = typeof(Enumerable)
                .GetMethod(nameof(Enumerable.ToList))?
                .MakeGenericMethod(target);
            var typedValues = toListMethod.Invoke(null, new object[] { typedEnumerable });

            return typedValues;
        }
    }
}
