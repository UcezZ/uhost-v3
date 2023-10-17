using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Properties;
using static System.Console;
using static Uhost.Core.Common.Tools;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Core.Extensions
{
    public static class CommonExtensions
    {
        /// <summary>
        /// Changes any property of <paramref name="obj"/> using lambda <paramref name="action"/>.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="obj">Source object instance.</param>
        /// <param name="action">Target action to be performed.</param>
        /// <returns>Changed object.</returns>
        public static T Having<T>(this T obj, Action<T> action)
        {
            action.Invoke(obj);
            return obj;
        }

        /// <summary>
        /// Преобразует объект в <see cref="JToken"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static JToken ToJToken(this object data)
        {
            return JToken.FromObject(data);
        }

        /// <summary>
        /// Сериализация объекта в JSON
        /// </summary>
        /// <param name="data">Объект для сериализации</param>
        /// <param name="formatting">Форматирование</param>
        /// <returns></returns>
        public static string ToJson(this object data, Formatting formatting = Formatting.None)
        {
            return data is JToken jToken
                ? jToken.ToString(formatting)
                : JsonConvert.SerializeObject(
                    data,
                    SerializerSettings
                        .Having(e => e.Formatting = formatting)
                        .Having(e => e.DateFormatString = DateTimeExtensions.DateTimeApiFmt)
                );
        }

        public static bool TryReadAsJson<TObject>(this FileInfo fileInfo, out TObject obj, out Exception exception)
        {
            if (!fileInfo.Exists)
            {
                obj = default;
                exception = new FileNotFoundException(default, fileInfo.FullName);
                return false;
            }

            try
            {
                using (var stream = fileInfo.OpenRead())
                using (var reader = new StreamReader(stream))
                using (var jReader = new JsonTextReader(reader))
                {
                    var serialiser = new JsonSerializer();
                    obj = serialiser.Deserialize<TObject>(jReader);
                }

                exception = default;
                return obj != null;
            }
            catch (Exception e)
            {
                exception = e;
                obj = default;
                return false;
            }
        }

        /// <summary>
        /// Отладочная сериализация объекта в JSON
        /// <br/>
        /// Пример выходного файла: Dump_20230220-105613_User.json
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="name">Имя переменной. Использовать с nameof()</param>
        /// <param name="gzip">Сжать дамп с помощью GZip</param>
        public async static Task<string> DumpAsync(this object data, string name = null, bool gzip = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = $"Dump_{data?.GetType().Name}_{DateTime.Now.ToFileFmt()}.json";
            }
            else
            {
                name = $"Dump_{data?.GetType().Name}_{name}_{DateTime.Now.ToFileFmt()}.json";
            }

            foreach (var ch in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(ch, '_');
            }

            try
            {
                try
                {
                    return await WriteDumpAsync(name, data, gzip);
                }
                catch
                {
                    return await WriteDumpAsync($"/tmp/{name}", data, gzip);
                }
            }
            catch (Exception e)
            {
                await Out.WriteLineAsync($"Failed to save dump: {e.Message}\r\n{e.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// Отладочная сериализация объекта в JSON
        /// <br/>
        /// Пример выходного файла: Dump_20230220-105613_User.json
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="name">Имя переменной. Использовать с nameof()</param>
        /// <param name="gzip">Сжать дамп с помощью GZip</param>
        public static string Dump(this object data, string name = null, bool gzip = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = $"Dump_{data?.GetType().Name}_{DateTime.Now.ToFileFmt()}.json";
            }
            else
            {
                name = $"Dump_{data?.GetType().Name}_{name}_{DateTime.Now.ToFileFmt()}.json";
            }

            foreach (var ch in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(ch, '_');
            }

            try
            {
                try
                {
                    return WriteDump(name, data, gzip);
                }
                catch
                {
                    return WriteDump($"/tmp/{name}", data, gzip);
                }
            }
            catch (Exception e)
            {
                WriteLine("Failed to save dump: {0}\r\n{1}", e.Message, e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Приведение JToken или объекта к заданному типу
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="input">"сырые" данные</param>
        /// <param name="value">Выходные данные</param>
        /// <param name="exception">Исключение</param>
        /// <returns>true если приведение успешно, иначе false</returns>
        public static bool TryCastTo<T>(this object input, out T value, out Exception exception)
        {
            if (input is T nothingToDo)
            {
                value = nothingToDo;
                exception = null;
                return true;
            }

            if (input != null)
            {
                try
                {
                    value = (T)input;
                    exception = null;
                    return true;
                }
                catch { }

                if (input is string str && str.TryParseJson(out var jToken1))
                {
                    input = jToken1;
                }
                else if (input.ToString().TryParseJson(out var jToken2))
                {
                    input = jToken2;
                }

                if (input is JToken jToken)
                {
                    try
                    {
                        value = jToken.ToObject<T>();
                        exception = null;
                        return true;
                    }
                    catch (Exception e)
                    {
                        value = default;
                        exception = e;
                        return false;
                    }
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        try
                        {
                            value = (T)converter.ConvertFromString(input.ToString());
                            exception = null;
                            return value != null;
                        }
                        catch (Exception e)
                        {
                            value = default;
                            exception = e;
                            return false;
                        }
                    }
                }
            }

            value = default;
            exception = null;
            return false;
        }

        /// <summary>
        /// Приведение JToken или объекта к заданному типу
        /// </summary>
        /// <typeparam name="T">Целевой тип</typeparam>
        /// <param name="input">"сырые" данные</param>
        /// <param name="value">Выходные данные</param>
        /// <returns>true если приведение успешно, иначе false</returns>
        public static bool TryCastTo<T>(this object input, out T value) =>
            input.TryCastTo(out value, out _);

        /// <inheritdoc cref="TranslateEnumValue{T}(T?)"/>
        public static string TranslateEnum<T>(this T value) where T : struct, Enum =>
            ((T?)value).TranslateEnumValue();

        /// <summary>
        /// Преобразует значение перечисления в человекопонятный текст
        /// </summary>
        /// <typeparam name="T">Перечисление</typeparam>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        public static string TranslateEnumValue<T>(this T? value) where T : struct, Enum
        {
            if (!typeof(T).IsEnum)
            {
                return EnumTranslations.Undefined;
            }

            if (value is Rights right)
            {
                return RightNames[right];
            }

            var key = $"{typeof(T).FullName}_{value}";

            return EnumTranslations.ResourceManager.GetString(key) ?? EnumTranslations.UndefinedFmt.Format(key);
        }

        /// <summary>
        /// Возвращает размер данных в понятном виде
        /// </summary>
        /// <param name="size">Размер данных</param>
        /// <returns></returns>
        public static string ToHumanSize(this int size)
        {
            string sizeStr, unit;

            if (size < 1 << 10)
            {
                sizeStr = size.ToString();
                unit = "B";
            }
            else if (size < 1 << 20)
            {
                sizeStr = Math.Round((double)size / (1 << 10), 2).ToString();
                unit = "KiB";
            }
            else if (size < 1 << 30)
            {
                sizeStr = Math.Round((double)size / (1 << 20), 2).ToString();
                unit = "MiB";
            }
            else
            {
                sizeStr = Math.Round((double)size / (1 << 30), 2).ToString();
                unit = "GiB";
            }

            if (sizeStr.Length > 4)
            {
                sizeStr = sizeStr[..4].Trim(',', '.');
            }

            return $"{sizeStr} {unit}";
        }

        public static bool TryGetValue<TKey, TValue, TOutValue>(this IDictionary<TKey, TValue> dict, TKey key, out TOutValue value) where TOutValue : TValue
        {
            if (dict.TryGetValue(key, out var obj) && obj is TOutValue outValue)
            {
                value = outValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public static object ToDetailedDataObject(this Exception exception)
        {
            return new
            {
                exception.Message,
                exception.StackTrace,
                exception.Source,
                exception.Data,
                Inner = exception.InnerException?.ToDetailedDataObject()
            };
        }

        public static string ToHexString(this byte[] buffer)
        {
            return buffer.Select(b => b.ToString("x2")).Concat();
        }
    }
}
