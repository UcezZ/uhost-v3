using Sentry;
using Sentry.Protocol;
using System;

namespace Uhost.Core.Config
{
    public sealed class SentryConfigWrapper
    {
        public string Dsn { get; set; }
        public bool Debug { get; set; }
        public SentryLevel DiagnosticsLevel { get; set; }
        public bool AttachStacktrace { get; set; }

        public void Configure(SentryOptions options)
        {
            options.Dsn = new Dsn(Dsn);
            options.Debug = Debug;
            options.DiagnosticsLevel = DiagnosticsLevel;
            options.AttachStacktrace = AttachStacktrace;
            options.Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        }
    }
}
