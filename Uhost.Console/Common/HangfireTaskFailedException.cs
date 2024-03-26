using Hangfire.States;
using System;

namespace Uhost.Console.Common
{
    public class HangfireTaskFailedException : Exception
    {
        public HangfireTaskFailedException(FailedState failedState, ElectStateContext context)
            : base($"Hangfire job {context.BackgroundJob?.Job?.Method?.DeclaringType?.FullName}.{context.BackgroundJob?.Job?.Method?.Name} has been failed", failedState.Exception)
        {
            Data["JobId"] = context.BackgroundJob?.Id;
            Data["JobMethod"] = $"{context.BackgroundJob?.Job?.Method?.DeclaringType?.FullName}.{context.BackgroundJob?.Job?.Method?.Name}";
            Data["JobMethodArgs"] = context.BackgroundJob?.Job?.Args;
            Data["ServerId"] = failedState.ServerId;
        }
    }
}
