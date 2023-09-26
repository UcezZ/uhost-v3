using System;

namespace Uhost.Core.Extensions
{
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// <inheritdoc cref="IServiceProvider.GetService(Type)"/>
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="provider"></param>
        /// <returns>Service of type <typeparamref name="TService"/> or null</returns>
        public static TService GetService<TService>(this IServiceProvider provider)
        {
            var service = provider.GetService(typeof(TService));

            if (service != null)
            {
                return (TService)service;
            }
            else
            {
                return default;
            }
        }
    }
}
