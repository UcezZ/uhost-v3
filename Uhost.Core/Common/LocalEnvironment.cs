using Microsoft.Extensions.Hosting;
using System;

namespace Uhost.Core.Common
{
    public static class LocalEnvironment
    {
        /// <summary>
        /// Среда - отладочная
        /// </summary>
        public static bool IsDev { get; }

        static LocalEnvironment()
        {
            IsDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development;
        }
    }
}
