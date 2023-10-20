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
    public sealed class SerializableTask
    {
        [JsonProperty]
        public string ServiceType { get; private set; }
        [JsonProperty]
        public string MethodName { get; private set; }
        [JsonProperty]
        public IEnumerable<string> ArgumentTypes { get; private set; }
        [JsonProperty]
        public IEnumerable<object> ArgumentValues { get; private set; }

        private SerializableTask() { }

        /// <summary>
        /// Создаёт сериализованную задачу из выражения.
        /// </summary>
        /// <typeparam name="T">Тип объекта, содержащеего метод.</typeparam>
        /// <param name="expression">Выражение. Может содержать только один метод.</param>
        /// <returns></returns>
        public static SerializableTask Create<T>(Expression<Action<T>> expression)
        {
            var body = expression.Body as MethodCallExpression;
            var values = body.Arguments.Select(e => e as ConstantExpression);

            if (values.Any(e => e is null))
            {
                throw new ArgumentException("Bad expression", nameof(expression));
            }

            return new SerializableTask
            {
                ServiceType = typeof(T).FullName,
                MethodName = body.Method.Name,
                ArgumentTypes = values.Select(e => e.Type.FullName),
                ArgumentValues = values.Select(e => e.Value)
            };
        }

        /// <summary>
        /// Создаёт сериализованную задачу из JSON.
        /// </summary>
        /// <param name="json">JSON</param>
        /// <returns></returns>
        public static SerializableTask FromJson(string json)
        {
            if (!json.TryCastTo<SerializableTask>(out var task))
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
        public static bool TryParseJson(string json, out SerializableTask task)
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

        public override string ToString()
        {
            return $"{ServiceType}.{MethodName}({ArgumentValues.Join(", ")})";
        }
    }
}
