using Microsoft.Extensions.DependencyInjection;
using System;

namespace Uhost.Console.Commands
{
    /// <summary>
    /// Базовый класс консольной команды
    /// </summary>
    public abstract class BaseCommand
    {
        private IServiceProvider _provider;

        protected TService GetService<TService>() => _provider.GetService<TService>();

        protected TService GetRequiredService<TService>() => _provider.GetRequiredService<TService>();

        protected object GetService(Type serviceType) => _provider.GetService(serviceType);

        protected object GetRequiredService(Type serviceType) => _provider.GetRequiredService(serviceType);

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
