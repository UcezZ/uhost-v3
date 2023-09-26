using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.States;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Services.HangfireExecutor;

namespace Uhost.Core.Services.HangfireScheduler
{
    public class HangfireSchedulerService : IHangfireSchedulerService
    {
        private readonly JobStorage _storage;
        private readonly BackgroundJobClient _client;

        public HangfireSchedulerService()
        {
            _storage = new PostgreSqlStorage(CoreSettings.SqlConnectionString);
            _client = new BackgroundJobClient(_storage);
        }

        public void ScheduleTest()
        {
            _client.Create<IHangfireExecutorService>(e => System.Console.WriteLine(GetType().ToJson(Newtonsoft.Json.Formatting.Indented)), new EnqueuedState(HangfireQueues.Default));
        }
    }
}
