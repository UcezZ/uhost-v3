using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UAParser;

namespace Uhost.Core.Extensions
{
    public static class HttpContextExtensions
    {
        private static readonly IPAddress _localhost = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private static readonly Parser _uaParser = Parser.GetDefault();

        /// <summary>
        /// Получает IP клиента из <see cref="HttpContext"/>
        /// </summary>
        public static IPAddress ResolveClientIp(this HttpContext context)
        {
            var ip = _localhost;

            if (context?.Connection?.RemoteIpAddress != null && !context.Connection.RemoteIpAddress.Equals(_localhost))
            {
                ip = context.Connection.RemoteIpAddress;
            }

            if (IPAddress.TryParse(context.GetHeaderValueAs<string>("X-Forwarded-For")?.SplitCsv().FirstOrDefault()?.Split(':').FirstOrDefault(), out var xForwarded) && !xForwarded.Equals(_localhost))
            {
                ip = xForwarded;
            }

            if (IPAddress.TryParse(context.GetHeaderValueAs<string>("REMOTE_ADDR")?.Split(':').FirstOrDefault(), out var remote) && !remote.Equals(_localhost))
            {
                ip = remote;
            }

            return ip;
        }

        public static T GetHeaderValueAs<T>(this HttpContext context, string headerName)
        {
            if (context?.Request?.Headers != null && context.Request.Headers.TryGetValue(headerName, out var values))
            {
                var rawValues = values.ToString();

                if (!string.IsNullOrWhiteSpace(rawValues))
                {
                    return (T)Convert.ChangeType(rawValues, typeof(T));
                }
            }

            return default;
        }

        public static IEnumerable<string> SplitCsv(this string csvList)
        {
            return csvList?
                .TrimEnd(',')
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim()) ?? Array.Empty<string>();
        }

        /// <summary>
        /// Получает информацию о клиенте по user-agent
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string ResolveClientInfo(this HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgent))
            {
                return string.Empty;
            }

            var info = _uaParser.Parse(userAgent);

            var browserVersion = $"{info.UA.Major}.{info.UA.Minor}.{info.UA.Patch}".Trim('.');
            var osVersion = $"{info.OS.Major}.{info.OS.Minor}.{info.OS.Patch}.{info.OS.PatchMinor}".Trim('.');


            return $"{info.UA.Family} {browserVersion}, {info.OS.Family} {osVersion}".TrimAll();
        }
    }
}
