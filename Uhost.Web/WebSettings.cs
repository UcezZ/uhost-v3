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

        static WebSettings() => CoreSettings.Load(typeof(WebSettings));
    }
}
