using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Extensions;

namespace Uhost.Core.Services
{
    /// <summary>
    /// Логгер
    /// <br/>
    /// Поддерживает асинхронную запись и запись в несколько <see cref="TextWriter"/>
    /// </summary>
    public sealed class LogWriter : IDisposable
    {
        /// <summary>
        /// Уровень события лога
        /// </summary>
        public enum Severity
        {
            /// <summary>
            /// Сообщение отладки
            /// </summary>
            Debug = 0,

            /// <summary>
            /// Информация
            /// </summary>
            Info = 1,

            /// <summary>
            /// Предупреждение
            /// </summary>
            Warn = 2,

            /// <summary>
            /// Ошибка
            /// </summary>
            Error = 3
        }

        private readonly ICollection<TextWriter> _writers;
        private readonly Severity _minSeverity;

        /// <summary>
        /// <inheritdoc cref="LogWriter"/>
        /// <br/>
        /// Конструктор с одним <see cref="TextWriter"/>
        /// </summary>
        /// <param name="writer"><see cref="TextWriter"/></param>
        /// <param name="minSeverity">Минимальный уровень сообщения для вывода</param>
        public LogWriter(TextWriter writer, Severity minSeverity = Severity.Info) : this(new List<TextWriter> { writer }, minSeverity) { }

        /// <summary>
        /// <inheritdoc cref="LogWriter"/>
        /// <br/>
        /// Конструктор с коллекцией <see cref="TextWriter"/>
        /// </summary>
        /// <param name="writers">Коллекция <see cref="TextWriter"/></param>
        /// <param name="minSeverity">Минимальный уровень сообщения для вывода</param>
        public LogWriter(IEnumerable<TextWriter> writers, Severity minSeverity = Severity.Info)
        {
            _writers = writers.ToList();
            _minSeverity = minSeverity;
        }

        /// <summary>
        /// Синхронно пишет сообщение во все <see cref="TextWriter"/>
        /// </summary>
        public void WriteLine(string message, Severity severity, params object[] args)
        {
            if (severity >= _minSeverity)
            {
                _writers.ForEach(e =>
                {
                    e.WriteLine($"[{DateTime.Now.ToLogFmt()}] [{severity}] {message}", args);
                    e.Flush();
                });
            }
        }

        /// <summary>
        /// Асинхронно пишет сообщение во все <see cref="TextWriter"/>
        /// </summary>
        public async Task WriteLineAsync(string message, Severity severity, params object[] args)
        {
            if (severity >= _minSeverity)
            {
                await Task.WhenAll(_writers.Select(async e =>
                {
                    await e.WriteLineAsync(string.Format($"[{DateTime.Now.ToLogFmt()}] [{severity}] {message}", args));
                    await e.FlushAsync();
                }));
            }
        }

        public void Dispose()
        {
            _writers.Dispose();
            _writers.Clear();
        }
    }
}
