using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Uhost.Core.Extensions;

namespace Uhost.Core.Common
{
    /// <summary>
    /// Сериализованная задача.
    /// </summary>
    public sealed class SerializedTask
    {
        [JsonProperty]
        public string ServiceType { get; private set; }
        [JsonProperty]
        public string MethodName { get; private set; }
        [JsonProperty]
        public IEnumerable<string> ArgumentTypes { get; private set; }
        [JsonProperty]
        public IEnumerable<object> ArgumentValues { get; private set; }

        private SerializedTask() { }

        /// <summary>
        /// Создаёт сериализованную задачу из выражения.
        /// </summary>
        /// <typeparam name="T">Тип объекта, содержащеего метод.</typeparam>
        /// <param name="expression">Выражение. Может содержать только один метод.</param>
        /// <returns></returns>
        public static SerializedTask Create<T>(Expression<Action<T>> expression)
        {
            var body = expression.Body as MethodCallExpression;
            var values = body.Arguments.Select(e => e.GetValue());

            if (values.Any(e => e is null))
            {
                throw new ArgumentException("Bad expression", nameof(expression));
            }

            return new SerializedTask
            {
                ServiceType = typeof(T).FullName,
                MethodName = body.Method.Name,
                //ArgumentTypes = values.Select(e => e?.Type.FullName),
                ArgumentTypes = body.Method.GetParameters().Select(e => e.ParameterType.FullName),
                ArgumentValues = values
            };
        }

        /// <summary>
        /// Создаёт сериализованную задачу из JSON.
        /// </summary>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        public static SerializedTask FromJson(string json)
        {
            if (!json.TryCastTo<SerializedTask>(out var task))
            {
                throw new ArgumentException("Bad json data", nameof(json));
            }

            return task;
        }

        /// <summary>
        /// Создаёт сериализованную задачу из JSON.
        /// </summary>
        /// <param name="json">JSON</param>
        /// <param name="task">Задача</param>
        /// <returns></returns>
        public static bool TryParseJson(string json, out SerializedTask task)
        {
            return json.TryCastTo(out task);
        }

        /// <summary>
        /// Вызывает метод, используя внедрение зависимостей.
        /// </summary>
        /// <param name="provider">Провайдер сервисов.</param>
        public void Invoke(IServiceProvider provider)
        {
            var targetType = Type.GetType(ServiceType, true);
            var values = Tools.ParallelSelect(ArgumentTypes.Select(Type.GetType), ArgumentValues)
               .Select(e => e.Value2.TryCastTo(e.Value1, out var value) ? value : null)
               .ToArray();
            var targetMethod = targetType.GetMethods().FirstOrDefault(e => e.Name == MethodName && e.GetParameters().Length == values.Length);

            if (targetMethod == null)
            {
                throw new InvalidOperationException("Bad target method");
            }

            var service = provider.GetRequiredService(targetType);

            targetMethod.Invoke(service, values);
        }

        /// <summary>
        /// Вызывает метод, используя внедрение зависимостей.
        /// </summary>
        /// <param name="serviceResolver">Метод получения сервиса.</param>
        public void Invoke(Func<Type, object> serviceResolver)
        {
            var targetType = Type.GetType(ServiceType, true);
            var types = ArgumentTypes.Select(Type.GetType);
            var values = Tools.ParallelSelect(types, ArgumentValues)
               .Select(e => e.Value2.TryCastTo(e.Value1, out var value) ? value : null)
               .ToArray();
            var targetMethod = targetType.GetMethods().FirstOrDefault(e => e.Name == MethodName && e.GetParameters().Length == values.Length);

            if (targetMethod == null)
            {
                throw new InvalidOperationException("Bad target method");
            }

            var service = serviceResolver.Invoke(targetType);

            targetMethod.Invoke(service, values);
        }

        public override string ToString()
        {
            return $"{ServiceType}.{MethodName}({ArgumentValues.Join(", ")})";
        }
    }
}
