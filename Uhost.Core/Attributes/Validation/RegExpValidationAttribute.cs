using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Uhost.Core.Attributes.Validation
{
    /// <summary>
    /// Валидация атрибута при помощи регулярного выражения с возможностью задавать параметры для регулярного выражения 
    /// </summary>
    public class RegExpValidationAttribute : RegularExpressionAttribute
    {
        private readonly RegexOptions _options;

        /// <summary>
        /// <inheritdoc cref="RegExpValidationAttribute"/>
        /// </summary>
        /// <param name="pattern">Регулярное выражение</param>
        /// <param name="options">Параметры</param>
        public RegExpValidationAttribute(string pattern, RegexOptions options = RegexOptions.None) : base(pattern)
        {
            _options = options;
        }

        public override bool IsValid(object value)
        {
            // Старая првоерка
            if (_options == RegexOptions.None)
            {
                return base.IsValid(value);
            }

            // Проверка строки на соответствие регулярному выражению
            var stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);

            if (string.IsNullOrEmpty(stringValue))
            {
                return true;
            }

            var matchRes = Regex.Match(stringValue, Pattern, _options);

            return matchRes.Success && matchRes.Index == 0 && matchRes.Length == stringValue.Length;
        }
    }
}
