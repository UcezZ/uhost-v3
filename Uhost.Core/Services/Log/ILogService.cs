using System;
using System.Collections.Generic;
using static Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Log
{
    public interface ILogService : IDisposable
    {
        IDictionary<Events, string> AllEvents { get; }

        void Add(Events ev, object data = null, bool writeToStdOut = false);
    }
}
