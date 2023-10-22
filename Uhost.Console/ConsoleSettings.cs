using System;
using Uhost.Core;

namespace Uhost.Console
{
    static class ConsoleSettings
    {
        /// <summary>
        /// Количество потоков выполнения задач
        /// </summary>
        public static int TaskExecutorWorkThreads { get; private set; } = Environment.ProcessorCount;

        static ConsoleSettings() => CoreSettings.Load(typeof(ConsoleSettings));
    }
}
