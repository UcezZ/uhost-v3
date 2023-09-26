using System.Reflection;
using System.Text.RegularExpressions;

namespace Uhost.Core.Common
{
    /// <summary>
    /// Представляет набор сведений о маршруте и методе контроллера
    /// </summary>
    public class RouteInfo
    {
        public Regex UrlRegex { get; set; }
        public string HttpMethod { get; set; }
        public MethodInfo ControllerMethod { get; set; }

        public override string ToString() => $"{HttpMethod} {UrlRegex}";
    }
}
