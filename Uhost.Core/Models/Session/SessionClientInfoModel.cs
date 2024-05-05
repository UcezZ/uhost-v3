using Microsoft.AspNetCore.Http;
using Uhost.Core.Extensions;

namespace Uhost.Core.Models.Session
{
    public sealed class SessionClientInfoModel
    {
        public string IPAddress { get; set; }

        public string Client { get; set; }

        public SessionClientInfoModel() { }

        public SessionClientInfoModel(HttpContext context)
        {
            IPAddress = context.ResolveClientIp()?.ToString();
            Client = context.ResolveClientInfo();
        }
    }
}
