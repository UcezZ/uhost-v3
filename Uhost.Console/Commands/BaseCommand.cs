using System;
using Uhost.Core.Extensions;

namespace Uhost.Console.Commands
{
    /// <summary>
    /// Интерфейс консольной команды
    /// </summary>
    public abstract class BaseCommand
    {
        private IServiceProvider _provider;

        protected TService GetService<TService>() => _provider.GetService<TService>();

        /// <summary>
        /// Добавляет провайдер сервисов DI
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public BaseCommand UseServiceProvider(IServiceProvider provider)
        {
            _provider = provider;

            return this;
        }

        /// <summary>
        /// Запуск команды
        /// </summary>
        public abstract void Run();
    }
}
