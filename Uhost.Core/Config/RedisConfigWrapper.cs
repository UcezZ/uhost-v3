using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Uhost.Core.Extensions;

namespace Uhost.Core.Config
{
    /// <summary>
    /// Universal wrapper for redis configuration to use JSON deserialization
    /// </summary>
    public sealed class RedisConfigWrapper
    {
        private int _poolSize;
        private int _workCount;

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

        /// <inheritdoc cref="RedisConfiguration.PoolSize"/>
        public int PoolSize { get => _poolSize < 1 ? 32 : _poolSize; set => _poolSize = value; }

        /// <inheritdoc cref="RedisConfiguration.WorkCount"/>
        public int WorkCount { get => _workCount < 1 ? 32 : _workCount; set => _workCount = value; }

        public static implicit operator ConfigurationOptions(RedisConfigWrapper obj)
        {
            var endPoints = obj.EndPoints
                .Select(e => e.TryParseEndPoint(out var endPoint) ? endPoint : null)
                .Where(e => e != null)
                .ToList();

            return new ConfigurationOptions
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
        }

        public static implicit operator RedisConfiguration(RedisConfigWrapper obj)
        {
            var endPoints = obj.EndPoints
                .Select(e => e.TryParseEndPoint(out var endPoint) ? endPoint : null)
                .Where(e => e != null)
                .ToList();

            var redisHosts = endPoints
                .OfType<IPEndPoint>()
                .Select(e => new RedisHost { Host = e.Address.ToString(), Port = e.Port })
                .Concat(endPoints
                    .OfType<DnsEndPoint>()
                    .Select(e => new RedisHost { Host = e.Host, Port = e.Port }))
                .ToArray();

            return new RedisConfiguration
            {
                AbortOnConnectFail = obj.AbortOnConnectFail,
                ConnectionSelectionStrategy = ConnectionSelectionStrategy.RoundRobin,
                ConnectTimeout = obj.ConnectTimeout,
                Hosts = redisHosts,
                Database = obj.DefaultDatabase,
                PoolSize = obj.PoolSize,
                WorkCount = obj.WorkCount,
                ServerEnumerationStrategy = new ServerEnumerationStrategy
                {
                    Mode = ServerEnumerationStrategy.ModeOptions.All,
                    TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
                    UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.IgnoreIfOtherAvailable
                },
                Ssl = obj.Ssl
            };
        }
    }
}
