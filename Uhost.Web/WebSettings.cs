using Hangfire;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Uhost.Core;

namespace Uhost.Web
{
    public class WebSettings
    {
        public sealed class JwtConfig
        {
            public string Key { get; set; }
            public string Issuer { get; set; }
            public string Audience { get; set; }
            public SecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
        }

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
