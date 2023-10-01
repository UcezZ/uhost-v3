using System;

namespace Uhost.Core.Common
{
    public class Timer : IDisposable
    {
        private readonly DateTime _start = DateTime.Now;

        public TimeSpan Span => DateTime.Now - _start;

        public int Milliseconds => (int)Span.TotalMilliseconds;

        void IDisposable.Dispose() { }
    }
}
