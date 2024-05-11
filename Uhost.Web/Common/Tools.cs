using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Uhost.Core.Common;
using Uhost.Core.Extensions;

namespace Uhost.Web.Common
{
    class Tools
    {
        public static IEnumerable<RouteInfo> Routes { get; }

        static Tools()
        {
            Routes = Program.Provider
                .GetRequiredService<IActionDescriptorCollectionProvider>()

                // все маршруты и методы контроллеров
                .ActionDescriptors.Items

                // описание маршрута контроллера
                .OfType<ControllerActionDescriptor>()

                // где есть маршрут
                .Where(e => e?.AttributeRouteInfo != null)

                // разворачиваем все методы
                .SelectMany(e => e.EndpointMetadata?.OfType<HttpMethodMetadata>().SelectMany(m => m.HttpMethods).Select(m => new
                {
                    ControllerActionDescriptor = e,
                    HttpMethod = m
                }))

                // регулярка маршрута, HTTP метод и метод контроллера
                .Select(e => new RouteInfo
                {
                    UrlRegex = new Regex($"^/{Regex.Replace(e.ControllerActionDescriptor.AttributeRouteInfo.Template, @"\{\w*\}", @"[\w\d_\-]+")}$"),
                    HttpMethod = e.HttpMethod,
                    ControllerMethod = e.ControllerActionDescriptor.MethodInfo
                })

                // commit
                .ToList();
        }
    }
}
