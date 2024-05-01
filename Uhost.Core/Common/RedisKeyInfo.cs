using StackExchange.Redis;
using System;

namespace Uhost.Core.Common
{
    internal readonly struct RedisKeyInfo
    {
        public readonly RedisKey RedisKey { get; }

        public readonly TimeSpan Expiry { get; }

        public RedisKeyInfo(RedisKey redisKey, TimeSpan? expiry)
        {
            RedisKey = redisKey;
            Expiry = expiry ?? default;
        }
    }
}
