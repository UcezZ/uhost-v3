using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Uhost.Core.Services.RedisSwitcher
{
    public interface IRedisSwitcherService
    {
        Task ExecuteAsync(Func<IDatabase, Task> action);
        Task<T> ExecuteAsync<T>(Func<IDatabase, Task<T>> action);
    }
}
