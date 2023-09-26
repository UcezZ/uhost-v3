using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using static System.Console;

namespace Uhost.Core
{
    public static class CoreSettings
    {
        private const string _jsonName = "appsettings.json";

        public static string RedisConnectionString { get; private set; }

        public static int RedisDatabase { get; private set; }

        public static string SqlConnectionString { get; private set; }

        public static int SqlCommandTimeoutSeconds { get; private set; }

        public static string PasswordSalt { get; private set; }

        static CoreSettings() => Load(typeof(CoreSettings));

        public static void Load(Type type)
        {
            IConfigurationRoot root;
            try
            {
                root = new ConfigurationBuilder()
                    .AddJsonFile(_jsonName)
                    .Build();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                throw;
            }

            var simpleProps = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(e => e.PropertyType.IsSerializable);
            var complexProps = type
                .GetProperties(BindingFlags.Public | BindingFlags.Static)
                .Where(e => !e.PropertyType.IsSerializable);

            foreach (var prop in simpleProps)
            {
                var value = root[prop.Name];

                if (value == null)
                {
                    throw new Exception($"NULL value got from key '{prop.Name}'");
                }
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(null, value);
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
                prop.SetValue(null, root.GetSection(prop.Name).Get(prop.PropertyType));

                if (prop.GetValue(null) == null)
                {
                    throw new Exception($"NULL section got from key '{prop.Name}'");
                }
            }
        }
    }
}
