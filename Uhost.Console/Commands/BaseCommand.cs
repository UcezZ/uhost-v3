using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using static System.Console;

namespace Uhost.Console.Commands
{
    /// <summary>
    /// Базовый класс консольной команды
    /// </summary>
    public abstract class BaseCommand
    {
        private readonly ICollection<string> _validationErrors = new List<string>();

        private IServiceProvider _provider;

        protected IServiceProvider Provider => _provider.CreateScope().ServiceProvider;

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
        protected abstract void Run();

        /// <summary>
        /// Валидация полей команды
        /// </summary>
        protected virtual bool Validate() => !_validationErrors.Any();

        /// <summary>
        /// Валидация и запуск команды
        /// </summary>
        public void ValidateAndRun()
        {
            if (Validate())
            {
                Run();
            }
            else
            {
                PrintValidationErrors();
            }
        }

        /// <summary>
        /// Добавляет ошибку валидации в коллекцию
        /// </summary>
        /// <param name="message"></param>
        protected void AddValidationError(string message)
        {
            _validationErrors.Add(message);
        }

        /// <summary>
        /// Выводит ошибки валидации в STDERR
        /// </summary>
        public void PrintValidationErrors()
        {
            _validationErrors.ForEach(Error.WriteLine);
        }
    }
}
