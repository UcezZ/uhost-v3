using FFMpegCore.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Uhost.Core.Attributes;
using Uhost.Core.Config;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Core
{
    public static class CoreSettings
    {
        private const string _jsonName = "appsettings.json";
        private static string _mediaServerUrl;

        public static string JsonName { get; } = Path.Combine(AppContext.BaseDirectory, _jsonName);

        public static IConfiguration Configuration { get; }

        public static IChangeToken ChangeTracking { get; }

        public static string SqlConnectionString { get; private set; }

        public static string SqlLogConnectionString { get; private set; }

        public static int SqlCommandTimeoutSeconds { get; private set; }

        public static string PasswordSalt { get; private set; }

        public static string FileStoragePath { get; private set; }

        public static string VideoStoragePath { get; private set; }

        public static string PublicUrl { get; private set; }

        public static string UploadsUrl { get; private set; }

        public static string VideosUrl { get; private set; }

        public static string HlsUrl { get; private set; }

        public static string VideoTokenSalt { get; private set; }

        [Unnecessary(DefaultValue = "")]
        public static string MediaServerUrl
        {
            get => string.IsNullOrWhiteSpace(_mediaServerUrl) ? PublicUrl : _mediaServerUrl;
            private set => _mediaServerUrl = value;
        }

        public static RedisConfigWrapper RedisConfig { get; private set; }

        public static SentryConfigWrapper SentryConfig { get; private set; }

        [Unnecessary]
        public static GraylogApiIntegration GraylogApi { get; private set; }

        public static SmtpClientWrapper SmtpConfig { get; private set; }

        /// <summary>
        /// Скорость кодирования. Необязательный параметр. Значение по умолчанию: <c>Slow</c>, может принимать значения: <c>VerySlow</c>, <c>Slower</c>, <c>Slow</c>, <c>Medium</c>, <c>Fast</c>, <c>Faster</c>, <c>VeryFast</c>, <c>SuperFast</c>, <c>UltraFast</c>
        /// </summary>
        [Unnecessary(DefaultValue = Speed.Slow)]
        public static Speed EncodingSpeed { get; private set; }

        /// <summary>
        /// ИД ролей для нового пользователя по умолчанию
        /// </summary>
        [Unnecessary]
        public static IEnumerable<int> NewUserRoleIds { get; private set; }

        static CoreSettings()
        {
            try
            {
                Configuration = new ConfigurationBuilder()
                    .AddJsonFile(JsonName)
                    .Build();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                throw;
            }

            Load(typeof(CoreSettings));
        }

        /// <summary>
        /// Загрузка значения по умолчанию для типа или значения по умолчанию из атрибута
        /// </summary>
        /// <param name="prop"></param>
        private static void LoadIfNull(PropertyInfo prop)
        {
            var attribute = prop.CustomAttributes
                .FirstOrDefault(e => e.AttributeType == typeof(UnnecessaryAttribute))?
                .ToAttributeInstance<UnnecessaryAttribute>();

            if (attribute != null)
            {
                try
                {
                    if (attribute.DefaultValue != null && attribute.DefaultValue.TryCastTo(prop.PropertyType, out var casted))
                    {
                        prop.SetValue(null, casted);
                    }
                    else
                    {
                        prop.SetValue(null, prop.PropertyType.Instantiate());
                    }
                }
                catch (Exception e)
                {
                    Error.WriteLine($"[WARN] Failed to instantiate unnecessary field '{prop.Name}' of type '{prop.PropertyType.FullName}': {e.Message}");
                }
            }
            else
            {
                throw new Exception($"NULL value got from key '{prop.Name}'");
            }
        }

        /// <summary>
        /// Заполнение класса конфигурации из конфигурационного файла
        /// </summary>
        /// <param name="type">typeof целевого статического класса</param>
        public static void Load(Type type)
        {
            var simpleProps = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(e => e.PropertyType.IsSerializable && e.CanWrite);
            var complexProps = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(e => !e.PropertyType.IsSerializable && e.CanWrite);

            foreach (var prop in simpleProps)
            {
                var value = Configuration[prop.Name];

                if (value == null)
                {
                    LoadIfNull(prop);
                }
                else if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(null, value?.ToString());
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(prop.PropertyType);

                    if (converter == null)
                    {
                        throw new NotSupportedException($"Unconvertible type got: '{prop.PropertyType.FullName}'");
                    }

                    prop.SetValue(null, converter.ConvertFromString(value));
                }
            }

            foreach (var prop in complexProps)
            {
                var value = Configuration.GetSection(prop.Name).Get(prop.PropertyType);

                if (value == null)
                {
                    LoadIfNull(prop);
                }
                else
                {
                    prop.SetValue(null, value);
                }
            }
        }
    }
}
