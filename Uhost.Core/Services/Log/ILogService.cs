using System;
using static Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Log
{
    public interface ILogService : IDisposable, IAsyncDisposable
    {
        void Add(Events ev, object data = null);
    }
}
