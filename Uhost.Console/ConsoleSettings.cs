using System;
using Uhost.Core;

namespace Uhost.Console
{
    static class ConsoleSettings
    {
        /// <summary>
        /// Количество потоков выполнения задач
        /// </summary>
        public static ushort TaskExecutorWorkThreads { get; private set; } = (ushort)Environment.ProcessorCount;

        static ConsoleSettings() => CoreSettings.Load(typeof(ConsoleSettings));
    }
}
