using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;

namespace Uhost.Core.Config
{
    /// <summary>
    /// Wrapper for <see cref="ConfigurationOptions"/> to use JSON deserialization
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

        /*"EndPoints": [
            "localhost:6379"
        ],
        "ConnectTimeout": 10000,
        "DefaultDatabase": 1,
        "HighPrioritySocketThreads": true,
        "IncludeDetailInExceptions": true,
        "ConnectRetry": 5,
        "SyncTimeout": 10000,
        "UseSsl": false,
        "AbortOnConnectFail": false,
        "ResponseTimeout": 10000*/
    }
}
