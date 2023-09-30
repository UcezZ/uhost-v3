using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Uhost.Web.Config
{
    public sealed class JwtConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public SecurityKey SecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
