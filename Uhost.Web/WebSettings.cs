using Hangfire;
using Uhost.Core;
using Uhost.Web.Config;

namespace Uhost.Web
{
    public partial class WebSettings
    {
        /// <summary>
        /// Время жизни токена в минутах
        /// </summary>
        public static int AuthTokenTtlMinutes { get; private set; }

        /// <summary>
        /// Параметры JWT
        /// </summary>
        public static JwtConfig Jwt { get; private set; }

        /// <summary>
        /// Параметры дэшборда Hangfire
        /// </summary>
        public static DashboardOptions HangfireDashboardOptions { get; private set; }

        static WebSettings() => CoreSettings.Load(typeof(WebSettings));
    }
}
