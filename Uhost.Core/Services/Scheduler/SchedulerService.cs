using RabbitMQ.Client;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using System;
using System.Text;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Models.File;
using Uhost.Core.Services.File;

namespace Uhost.Core.Services.Scheduler
{
    public sealed class SchedulerService : ISchedulerService
    {
        private readonly IQueueService _queue;

        public SchedulerService(IQueueService queue)
        {
            _queue = queue;
            _queue.RegisterQueue(TaskQueues.Conversion);
        }

        public void ScheduleTest()
        {
            _queue.Enqueue<IFileService>(e => Console.WriteLine(e.GetAll<FileViewModel>(new FileQueryModel()).ToJson(Newtonsoft.Json.Formatting.Indented)), TaskQueues.Conversion);
            _queue.Channel.BasicPublish(string.Empty, TaskQueues.Conversion, null, Encoding.UTF8.GetBytes("Жопа!"));
        }
    }
}
