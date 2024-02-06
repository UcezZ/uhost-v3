using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;

namespace Uhost.Console.Common
{
    public class HangfireJobActivator : JobActivator
    {
        private static IDictionary<Type, ServiceLifetime> _serviceLifeTimes;

        public static void Init(IServiceCollection services)
        {
            _serviceLifeTimes ??= services
                .DistinctBy(e => e.ServiceType)
                .ToDictionary(e => e.ServiceType, e => e.Lifetime);
        }

        private readonly IServiceProvider _provider;

        public HangfireJobActivator(IServiceProvider provider)
        {
            _provider = provider.CreateScope().ServiceProvider;
        }

        public override object ActivateJob(Type jobType)
        {
            if (_serviceLifeTimes.TryGetValue(jobType, out var lifeTime) && lifeTime == ServiceLifetime.Scoped)
            {
                using (var scope = _provider.CreateScope())
                {
                    return scope.ServiceProvider.GetRequiredService(jobType);
                }
            }

            return _provider.GetRequiredService(jobType);
        }
    }
}
