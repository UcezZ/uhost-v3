using Uhost.Core;

namespace Uhost.Convert
{
    static class ConvertSettings
    {
        /// <summary>
        /// Количество потоков выполнения конвертера
        /// </summary>
        public static int ConverterWorkThreads { get; private set; }

        static ConvertSettings() => CoreSettings.Load(typeof(ConvertSettings));
    }
}
