using System;

namespace Uhost.Core.Attributes
{
    /// <summary>
    /// Пометка параметра конфигурации как необязательного
    /// </summary>
    /// <remarks>
    /// При попытке заполнения параметра конфигурации из файла конфигурации не будет брошено исключение, если параметра нет в файле конфигурации или он указан как null
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UnnecessaryAttribute : Attribute
    {
        public UnnecessaryAttribute(object defaultValue = default)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Значение поля по умолчанию
        /// </summary>
        public object DefaultValue { get; }
    }
}
