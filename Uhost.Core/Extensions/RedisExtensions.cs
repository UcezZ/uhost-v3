using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace Uhost.Core.Extensions
{
    public static class RedisExtensions
    {
        private static readonly RedisValue _allPattern = "*";

        public static IEnumerable<RedisKey> GetKeys(this IConnectionMultiplexer conn, RedisValue? pattern, int? dbNum = null)
        {
            dbNum ??= CoreSettings.RedisConfig.DefaultDatabase;
            var servers = conn.GetServers();
            var server = servers.FirstOrDefault();
            var keys = server.Keys((int)dbNum, pattern ?? _allPattern);

            return keys;
        }

        public static IAsyncEnumerable<RedisKey> GetKeysAsync(this IConnectionMultiplexer conn, RedisValue? pattern, int? dbNum = null)
        {
            dbNum ??= CoreSettings.RedisConfig.DefaultDatabase;

            return conn.GetServers()
                .FirstOrDefault()
                .KeysAsync((int)dbNum, pattern ?? _allPattern);
        }
    }
}
