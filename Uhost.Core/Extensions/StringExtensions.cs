using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Uhost.Core.Extensions
{
    public static class StringExtensions
    {
        private const string _qwertyLat = "qwertyuiop[]asdfghjkl;'zxcvbnm,./QWERTYUIOP{}ASDFGHJKL:\"ZXCVBNM<>?";
        private const string _qwertyRus = "йцукенгшщзхъфывапролджэячсмитьбю.ЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ,";
        private const string _similarRus = "АВЕКМНОРСТУХавекмнорстух";
        private const string _similarLat = "ABEKMHOPCTYXabekmhopctyx";
        private const string _pgEscapeChars = "_%()";
        private const char _pgEscapeChar = '\\';
        private static readonly Regex _digitRegex = new Regex(@"\d{1,}");
        private static readonly Regex _whiteSpacesReplaceRegex = new Regex(@"\s+");

        /// <summary>
        /// Валидация строки Email-адреса
        /// </summary>
        /// <returns>true если адрес валидный</returns>
        public static bool IsValidEmailAddress(this string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <inheritdoc cref="string.Format(string, object[])"/>
        public static string Format(this string format, params object[] args) => string.Format(format, args);

        /// <inheritdoc cref="string.Join{T}(string, IEnumerable{T})"/>
        public static string Join<T>(this IEnumerable<T> values, string separator)
        {
            return string.Join(separator, values);
        }

        /// <summary>
        /// Обрезает пробелы на каждом слове
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TrimAll(this string text)
        {
            text = _whiteSpacesReplaceRegex.Replace(text, " ");

            return text.Trim();
        }

        /// <summary>
        /// Обёртка для <see cref="Enum.TryParse{TEnum}(string, out TEnum)"/>. Возвращает значение enum'а при удачном парсинге, иначе null
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TEnum? ParseEnum<TEnum>(this string str) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(str, out var parsed) ? parsed : null;
        }

        /// <summary>
        /// Проверяет строку на совпадение с регулярным выражением
        /// </summary>
        /// <param name="value">Строка</param>
        /// <param name="pattern">Регулярное выражение</param>
        /// <returns></returns>
        public static bool IsMatchesRegex(this string value, string pattern)
        {
            var regex = new Regex(pattern);
            return regex.IsMatch(value);
        }

        /// <summary>
        /// Ескейпинг сипволов для подстановки в SQL запрос
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string PostgresEscape(this string value)
        {
            foreach (var e in _pgEscapeChars)
            {
                value = value.Replace($"{e}", $"{_pgEscapeChar}{e}");
            }

            return value;
        }

        /// <summary>
        /// Переводит раскладку с русского на английский
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Qwertify(this string input)
        {
            return new string(
                input
                .ToUpper()
                .Select(e => _qwertyRus.Contains(e) ? _qwertyLat[_qwertyRus.IndexOf(e)] : e)
                .ToArray());
        }

        /// <summary>
        /// Переводит раскладку с английского на русский
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Deqwertify(this string input)
        {
            return new string(
                input
                .ToUpper()
                .Select(e => _qwertyLat.Contains(e) ? _qwertyRus[_qwertyLat.IndexOf(e)] : e)
                .ToArray());
        }

        /// <summary>
        /// Парсинд ИД из запроса
        /// </summary>
        /// <param name="input">Строка с ИД</param>
        /// <param name="parsed">Целочисленный ИД</param>
        /// <returns></returns>
        public static bool TryParsePositiveInt(this string input, out int parsed)
        {
            return int.TryParse(input, out parsed) && parsed > 0;
        }

        /// <summary>
        /// TryParse для коллекции
        /// </summary>
        /// <typeparam name="T">Получаемый тип</typeparam>
        /// <param name="input">Входная строка</param>
        /// <param name="delimiter">Разделитель</param>
        /// <param name="result">Выходные значения</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static bool TryParseMany<T>(this string input, char delimiter, out ICollection<T> result)
        {
            var converter = TypeDescriptor.GetConverter(typeof(T)) ?? throw new NotSupportedException($"Unconvertible type got: '{typeof(T).FullName}'");
            result = new List<T>();

            foreach (string value in input.Split(delimiter, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                try
                {
                    result.Add((T)converter.ConvertFromString(value));
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Попытка парса строки JSON в <see cref="JToken"/>
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <param name="jToken">Спаршенный <see cref="JToken"/></param>
        /// <returns>true если парсинг успешен, иначе false</returns>
        public static bool TryParseJson(this string input, out JToken jToken)
        {
            try
            {
                jToken = JToken.Parse(input);
                return true;
            }
            catch
            {
                jToken = default;
                return false;
            }
        }

        /// <summary>
        /// Транслитерация похожих символов на латиницу
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <returns></returns>
        public static string SimilarTransliterate(this string input)
        {
            return new string(input
                .ToUpper()
                .Select(e => _similarRus.Contains(e) ? _similarLat[_similarRus.IndexOf(e)] : e)
                .ToArray());
        }

        /// <summary>
        /// Транслитерация похожих символов на кириллицу
        /// </summary>
        /// <param name="input">Входная строка</param>
        /// <returns></returns>
        public static string SimilarBackTransliterate(this string input)
        {
            return new string(input
                .ToUpper()
                .Select(e => _similarLat.Contains(e) ? _similarRus[_similarLat.IndexOf(e)] : e)
                .ToArray());
        }

        public static string Concat<T>(this IEnumerable<T> values) =>
            string.Concat(values);

        public static bool TryParseEndPoint(this string input, out EndPoint endPoint)
        {
            var split = input.Split(':');

            if (split.Length != 2)
            {
                endPoint = default;

                return false;
            }

            if (!split[1].TryParsePositiveInt(out var port))
            {
                endPoint = default;

                return false;
            }

            if (IPAddress.TryParse(split[0], out var ip))
            {
                endPoint = new IPEndPoint(ip, port);
            }
            else
            {
                endPoint = new DnsEndPoint(split[0], port);
            }

            return true;
        }

        public static string ReverseString(this string input)
        {
            if (input == null)
            {
                return null;
            }

            return new string(input.Reverse().ToArray());
        }

        public static int EqualityFromStart(this string a, string b)
        {
            if (a == null || b == null)
            {
                return 0;
            }

            int i;

            for (i = 0; i < a.Length && i < b.Length && a[i] == b[i]; ++i) ;

            return i;
        }

        public static int EqualityFromEnd(this string a, string b)
        {
            if (a == null || b == null)
            {
                return 0;
            }

            return EqualityFromStart(a.ReverseString(), b.ReverseString());
        }

        public static int ParseDigits(this string input)
        {
            var match = _digitRegex.Match(input);

            if (match.Success)
            {
                return int.TryParse(match.Value, out var result) ? result : 0;
            }

            return 0;
        }
    }
}
