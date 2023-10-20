using System;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Core.Models.File;
using Uhost.Core.Services.File;

namespace Uhost.Core.Services.Task.Executor
{
    public class ExecutorService : IExecutorService
    {
        private readonly IFileService _fileService;

        public ExecutorService(IFileService fileService)
        {
            _fileService = fileService;
        }

        public void Test()
        {
            Console.WriteLine(_fileService.GetAll<FileShortViewModel>(new FileQueryModel()).Take(10).ToJson(Newtonsoft.Json.Formatting.Indented));
        }
    }
}
