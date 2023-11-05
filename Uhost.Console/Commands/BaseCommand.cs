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

        protected IServiceProvider Provider => _provider.CreateScope().ServiceProvider;

        protected TService GetService<TService>() => Provider.GetService<TService>();

        protected TService GetRequiredService<TService>() => Provider.GetRequiredService<TService>();

        protected object GetService(Type serviceType) => Provider.GetService(serviceType);

        protected object GetRequiredService(Type serviceType) => Provider.GetRequiredService(serviceType);

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
