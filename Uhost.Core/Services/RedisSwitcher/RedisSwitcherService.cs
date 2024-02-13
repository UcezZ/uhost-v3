using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Core.Services.RedisSwitcher
{
    /// <summary>
    /// Обёртка для Redis для отказоустойчивости подключения
    /// </summary>
    public class RedisSwitcherService : IRedisSwitcherService
    {
        private readonly List<ConnectionMultiplexer> _redis;

        public RedisSwitcherService()
        {
            _redis = CoreSettings.RedisConfig
                .AsConfigurationOptionsEnumerable()
                .Select(e => ConnectionMultiplexer.Connect(e))
                .ToList();
        }

        /// <summary>
        /// Выполнение асинхронного метода void
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task ExecuteAsync(Func<IDatabase, Task> action)
        {
            Exception ex = null;

            for (var i = 0; i < CoreSettings.RedisConfig.ConnectRetry; i++)
            {
                var db = _redis[0]?
                    .GetDatabase();

                try
                {
                    await action.Invoke(db);
                    return;
                }
                catch (Exception e)
                {
                    ex = e;
                    WriteLine($"[Redis switcher] Unable to invoke action on '{db?.Multiplexer?.Configuration?.Split(',').FirstOrDefault()}', rotating: {e.Message}");
                    _redis.Rotate();
                }
            }

            throw ex;
        }

        /// <summary>
        /// Выполнение асинхронного метода с возвратом значения
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(Func<IDatabase, Task<T>> action)
        {
            Exception ex = null;

            for (var i = 0; i < CoreSettings.RedisConfig.ConnectRetry; i++)
            {
                var db = _redis[0]?
                    .GetDatabase();

                try
                {
                    return await action.Invoke(db);
                }
                catch (Exception e)
                {
                    ex = e;
                    WriteLine($"[Redis switcher] Unable to invoke action on '{db?.Multiplexer?.Configuration?.Split(',').FirstOrDefault()}', rotating: {e.Message}");
                    _redis.Rotate();
                }
            }

            throw ex;
        }
    }
}
