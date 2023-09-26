using Hangfire;
using Hangfire.PostgreSql;
using System;
using Uhost.Core;

namespace Uhost.Convert
{
    class Program
    {
        private static bool _cancel;
        static void Main(string[] args)
        {
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(CoreSettings.SqlConnectionString);

            Console.CancelKeyPress += OnCancelKeyPress;

            using (var server = new BackgroundJobServer())
            {
                while (!_cancel)
                {
                    Console.ReadKey(true);
                }
            }
        }
        private static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            _cancel = true;
            e.Cancel = true;
        }
    }
}
