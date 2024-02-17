using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;

namespace Uhost.Core.Config
{
    /// <summary>
    /// Универсальный класс конфигурации Redis
    /// </summary>
    public sealed class RedisConfigWrapper
    {
        /// <inheritdoc cref="ConfigurationOptions.EndPoints"/>
        public IEnumerable<string> EndPoints { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.ConnectTimeout"/>
        public int ConnectTimeout { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.DefaultDatabase"/>
        public int DefaultDatabase { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.IncludeDetailInExceptions"/>
        public bool IncludeDetailInExceptions { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.ConnectRetry"/>
        public int ConnectRetry { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.SyncTimeout"/>
        public int SyncTimeout { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.Ssl"/>
        public bool Ssl { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.AbortOnConnectFail"/>
        public bool AbortOnConnectFail { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.ResponseTimeout"/>
        public int ResponseTimeout { get; set; }

        /// <inheritdoc cref="ConfigurationOptions.KeepAlive"/>
        public int KeepAlive { get; set; }

        public static implicit operator ConfigurationOptions(RedisConfigWrapper obj)
        {
            var endPoints = obj.EndPoints
                .Select(e => e.TryParseEndPoint(out var endPoint) ? endPoint : null)
                .Where(e => e != null)
                .ToList();

            var config = new ConfigurationOptions
            {
                EndPoints = new EndPointCollection(endPoints),
                ConnectTimeout = obj.ConnectTimeout,
                DefaultDatabase = obj.DefaultDatabase,
                IncludeDetailInExceptions = obj.IncludeDetailInExceptions,
                ConnectRetry = obj.ConnectRetry,
                SyncTimeout = obj.SyncTimeout,
                Ssl = obj.Ssl,
                AbortOnConnectFail = obj.AbortOnConnectFail,
                KeepAlive = obj.KeepAlive
            };

            return config;
        }

        public IEnumerable<ConfigurationOptions> AsConfigurationOptionsEnumerable()
        {
            return EndPoints
                .Select(e => e.TryParseEndPoint(out var endPoint) ? endPoint : null)
                .Where(e => e != null)
                .Select(e => new ConfigurationOptions
                {
                    EndPoints = new EndPointCollection(new[] { e }),
                    ConnectTimeout = ConnectTimeout,
                    DefaultDatabase = DefaultDatabase,
                    IncludeDetailInExceptions = IncludeDetailInExceptions,
                    ConnectRetry = ConnectRetry,
                    SyncTimeout = SyncTimeout,
                    Ssl = Ssl,
                    AbortOnConnectFail = AbortOnConnectFail,
                    KeepAlive = KeepAlive
                });
        }
    }
}
