using System;
using System.IO;
using Uhost.Core.Models.Graylog;

namespace Uhost.Core.Services.Graylog
{
    public interface IGraylogService : IDisposable
    {
        MemoryStream GetGraylogCsvStream(GraylogRestQueryModel query);
    }
}
