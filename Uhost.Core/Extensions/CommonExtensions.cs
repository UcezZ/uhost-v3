using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Uhost.Core.Properties;
using static System.Console;
using static Uhost.Core.Common.Tools;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Core.Extensions
{
    public static class CommonExtensions
    {
        private static readonly FileExtensionContentTypeProvider _extensionProvider = new FileExtensionContentTypeProvider();

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
        /// Клонирование объекта через JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T JsonClone<T>(this T obj)
        {
            return JToken.FromObject(obj).ToObject<T>();
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
        /// <param name="input">"сырые" данные</param>
        /// <param name="type">Целевой тип</param>
        /// <param name="value">Выходные данные</param>
        /// <param name="exception">Исключение</param>
        /// <returns>true если приведение успешно, иначе false</returns>
        public static bool TryCastTo(this object input, Type type, out object value, out Exception exception)
        {
            if (input.GetType() == type || input.GetType().IsAssignableFrom(type))
            {
                value = input;
                exception = null;
                return true;
            }

            if (input != null)
            {
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
                        value = jToken.ToObject(type);
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
                    var converter = TypeDescriptor.GetConverter(type);

                    if (converter != null)
                    {
                        try
                        {
                            value = converter.ConvertFromString(input.ToString());
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

        /// <summary>
        /// Приведение JToken или объекта к заданному типу
        /// </summary>
        /// <param name="input">"сырые" данные</param>
        /// <param name="type">Целевой тип</param>
        /// <param name="value">Выходные данные</param>
        /// <returns>true если приведение успешно, иначе false</returns>
        public static bool TryCastTo(this object input, Type type, out object value) =>
            input.TryCastTo(type, out value, out _);

        /// <inheritdoc cref="Translate{T}(T?)"/>
        public static string Translate<T>(this T value) where T : struct, Enum =>
            ((T?)value).Translate();

        /// <summary>
        /// Преобразует значение перечисления в человекопонятный текст
        /// </summary>
        /// <typeparam name="T">Перечисление</typeparam>
        /// <param name="value">Значение</param>
        /// <returns></returns>
        public static string Translate<T>(this T? value) where T : struct, Enum
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
            bool negate = size < 0;

            if (negate)
            {
                size = -size;
            }

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

            return negate
                ? $"-{sizeStr} {unit}"
                : $"{sizeStr} {unit}";
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

        public static Exception GetMostInnerException(this Exception exception)
        {
            while (exception?.InnerException != null)
            {
                exception = exception.InnerException;
            }

            return exception;
        }

        public static string ToHexString(this byte[] buffer)
        {
            return buffer.Select(b => b.ToString("x2")).Concat();
        }

        /// <summary>
        /// Заполняет свойства объекта типа <typeparamref name="T"/> из объекта <paramref name="source"/>
        /// </summary>
        /// <typeparam name="T">Целевой тип объекта</typeparam>
        /// <param name="obj">Объект назначения</param>
        /// <param name="source">Объект-источник</param>
        public static void LoadFrom<T>(this T obj, T source)
        {
            var props = typeof(T)
                .GetProperties()
                .Where(e => e.CanWrite && e.CanRead)
                .ToList();

            foreach (var prop in props)
            {
                var value = prop.GetValue(source);

                if (value != null || value != default)
                {
                    prop.SetValue(obj, value);
                }
            }

            props.Clear();
        }

        /// <summary>
        /// Преобразует объект в словарь свойств
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyFilter"></param>
        /// <returns></returns>
        public static Dictionary<string, object> ToPropertiesDictionary(this object obj, Func<PropertyInfo, bool> propertyFilter = default)
        {
            propertyFilter ??= e => true;
            var dictionary = new Dictionary<string, object>();
            var props = obj.GetType().GetProperties().Where(propertyFilter);

            foreach (var prop in props)
            {
                try
                {
                    dictionary[prop.Name] = prop.GetValue(obj);
                }
                catch
                {
                    dictionary[prop.Name] = null;
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Попытка удаления файла
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        public static bool TryDeleteIfExists(this FileInfo fileInfo)
        {
            try
            {
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Получает все ключи
        /// </summary>
        /// <param name="config"></param>
        /// <param name="parentKey"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllKeys(this IConfiguration config, string parentKey = null)
        {
            foreach (var child in config.GetChildren())
            {
                var currentKey = string.IsNullOrEmpty(parentKey) ? child.Key : $"{parentKey}:{child.Key}";
                var hasChildren = false;

                foreach (var key in child.GetAllKeys(currentKey))
                {
                    hasChildren = true;
                    yield return key;
                }

                if (!hasChildren)
                {
                    yield return currentKey;
                }
            }
        }

        /// <summary>
        /// Рекурсивное копирование конфигурации
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyTo(this IConfiguration source, IConfiguration destination)
        {
            foreach (var key in source.GetAllKeys())
            {
                destination[key] = source[key];
            }
        }

        /// <summary>
        /// Получает MIME из имени файла
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetContentType(this string fileName)
        {
            return _extensionProvider.TryGetContentType(fileName, out var mime)
                ? mime
                : "application/octet-stream";
        }
    }
}
