using Microsoft.AspNetCore.StaticFiles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using static System.Console;
using static Uhost.Core.Data.Entities.Right;

namespace Uhost.Core.Common
{
    public static class Tools
    {
        private const string _randomChars = "1234567890-qwertyuiopasdfghjklzxcvbnm.QWERTYUIOPASDFGHJKLZXCVBNM_";
        private static readonly FileExtensionContentTypeProvider _mimeProvider = new FileExtensionContentTypeProvider();

        internal static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings
        {
            MaxDepth = 512,
            CheckAdditionalContent = true,
            TypeNameHandling = TypeNameHandling.None,
            DateFormatString = DateTimeExtensions.DateTimeApiFmt,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// Словарь прав
        /// </summary>
        internal static Dictionary<Rights, string> RightNames { get; }

        static Tools()
        {
            using (var ctx = new PostgreSqlDbContext())
            {
                //RightNames = ctx.Rights.ToDictionary(e => (Rights)e.Id, e => e.Name);
                RightNames = Enum.GetValues<Rights>().ToDictionary(e => e, e => e.ToString());
            }
        }

        /// <summary>
        /// Пишет дамп объекта в файл
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="data">объект</param>
        /// <param name="gzip">сжать дамп в gzip</param>
        /// <returns></returns>
        public static async Task<string> WriteDumpAsync(string filename, object data, bool gzip)
        {
            if (gzip)
            {
                filename += ".gz";
            }

            using (var fs = new FileStream(filename, FileMode.Create))
            {
                using (var sw = new StreamWriter(gzip ? new GZipStream(fs, CompressionLevel.Optimal, true) : fs))
                {
                    await sw.WriteAsync(data.ToJson(Formatting.Indented));
                }

                await Out.WriteLineAsync($"Dump saved to \"{fs.Name}\"");

                return fs.Name;
            }
        }

        /// <summary>
        /// Пишет дамп объекта в файл
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="data">объект</param>
        /// <param name="gzip">сжать дамп в gzip</param>
        /// <returns></returns>
        public static string WriteDump(string filename, object data, bool gzip)
        {
            if (gzip)
            {
                filename += ".gz";
            }

            using (var fs = new FileStream(filename, FileMode.Create))
            {
                using (var sw = new StreamWriter(gzip ? new GZipStream(fs, CompressionLevel.Optimal, true) : fs))
                {
                    sw.Write(data.ToJson(Formatting.Indented));
                }

                WriteLine("Dump saved to \"{0}\"", fs.Name);

                return fs.Name;
            }
        }

        /// <summary>
        /// Перечисляет сущности в проекте
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> EnumerateEntities()
        {
            return Assembly
                .GetAssembly(typeof(BaseEntity))
                .GetTypes()
                .Where(e => e.MemberType == MemberTypes.TypeInfo && typeof(BaseEntity).IsAssignableFrom(e) && !e.IsAbstract)
                .OrderBy(e => e.Name);
        }

        /// <summary>
        /// Проверяет, существует ли сущность с таким названием в проекте
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool EntityExistsByName(string name)
        {
            return EnumerateEntities()
                .Any(e => e.Name == name);
        }

        /// <summary>
        /// Получает имя таблицы по типу сущности
        /// </summary>
        /// <param name="entityType">Тип сущности</param>
        /// <returns></returns>
        public static string GetEntityTableNameByEntityType(Type entityType)
        {
            var value = entityType?
                .CustomAttributes?
                .FirstOrDefault(e => e.AttributeType == typeof(TableAttribute))?
                .ConstructorArguments?
                .FirstOrDefault().Value;

            return value is string tableName ? tableName : null;
        }

        /// <summary>
        /// Как <see cref="Path.Combine(string[])"/>, но для URL
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string UrlCombine(params string[] parts)
        {
            var url = string.Join("/", parts.Select(e => e?.Replace('\\', '/').Trim().Trim(new[] { '/', '\\' })));

            return url;
        }

        /// <summary>
        /// Генерация случайных символов
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomChars(int length)
        {
            var r = new Random((int)DateTime.Now.Ticks);

            return new string(Enumerable.Range(0, length).Select(e => _randomChars[r.Next(_randomChars.Length)]).ToArray());
        }

        public static IEnumerable<ValuePair<T1, T2>> ParallelSelect<T1, T2>(IEnumerable<T1> values1, IEnumerable<T2> values2)
        {
            var enumerator1 = values1.GetEnumerator();
            var enumerator2 = values2.GetEnumerator();

            while (enumerator1.MoveNext() && enumerator2.MoveNext())
            {
                yield return new ValuePair<T1, T2>(enumerator1.Current, enumerator2.Current);
            }
        }

        public static IEnumerable<ValuePair<T1, T2>> ParallelSelect<T1, T2>(T1[] values1, T2[] values2)
        {
            for (var i = 0; i < values1.Length && i < values2.Length; i++)
            {
                yield return new ValuePair<T1, T2>(values1[i], values2[i]);
            }
        }

        public static string GetMimeFromName(string name)
        {
            return _mimeProvider.TryGetContentType(name, out var mime)
                ? mime :
                "application/octet-stream";
        }
    }
}
