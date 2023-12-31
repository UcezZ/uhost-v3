﻿using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Uhost.Core.Config;
using static System.Console;

namespace Uhost.Core
{
    public static class CoreSettings
    {
        private const string _jsonName = "appsettings.json";

        public static string JsonName { get; } = Path.Combine(AppContext.BaseDirectory, _jsonName);

        public static IConfiguration Configuration { get; }

        public static string SqlConnectionString { get; private set; }

        public static string SqlLogConnectionString { get; private set; }

        public static int SqlCommandTimeoutSeconds { get; private set; }

        public static string PasswordSalt { get; private set; }

        public static string FileStoragePath { get; private set; }

        public static string PublicUrl { get; private set; }

        public static string UploadsUrl { get; private set; }

        public static RedisConfigWrapper RedisConfig { get; private set; }

        public static SentryConfigWrapper SentryConfig { get; private set; }

        public static GraylogApiIntegration GraylogApi { get; private set; }

        public static RabbitMqClientOptions RabbitMqClientOptions { get; private set; }

        public static SmtpClientWrapper SmtpConfig { get; private set; }

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
                prop.SetValue(null, Configuration.GetSection(prop.Name).Get(prop.PropertyType));

                if (prop.GetValue(null) == null)
                {
                    throw new Exception($"NULL section got from key '{prop.Name}'");
                }
            }
        }
    }
}
