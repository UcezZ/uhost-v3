using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Uhost.Console.Common
{
    public class HangfireJobActivator : JobActivator
    {
        private readonly IServiceProvider _provider;

        public HangfireJobActivator(IServiceProvider provider)
        {
            _provider = provider.CreateScope().ServiceProvider;
        }

        public override object ActivateJob(Type jobType)
        {
            var scope = _provider.CreateScope();
            return scope.ServiceProvider.GetRequiredService(jobType);
        }
    }
}
