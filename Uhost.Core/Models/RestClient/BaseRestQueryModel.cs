using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Uhost.Core.Models.RestClient
{
    /// <summary>
    /// Базовая модель запроса к REST API
    /// </summary>
    public class BaseRestQueryModel
    {
        /// <summary>
        /// Параметры запроса. можно добавить коллекцию вв значения для передачи массива в URL
        /// </summary>
        protected IDictionary<string, object> UrlParameters { get; set; }

        public BaseRestQueryModel()
        {
            UrlParameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// URL запроса
        /// </summary>
        protected virtual string Url { get; set; }

        /// <summary>
        /// Метод HTTP
        /// </summary>
        protected virtual HttpMethod HttpMethod { get; set; }

        /// <summary>
        /// Запрос в <see cref="Uri"/>
        /// </summary>
        protected Uri RequestUri
        {
            get
            {
                var url = Url;

                if (UrlParameters != null && UrlParameters.Any())
                {
                    var urlParams = new List<KeyValuePair<string, object>>();

                    foreach (var param in UrlParameters)
                    {
                        // параметр-коллекция вида key=value1&key=value2&key=value3&...
                        if (param.Value is IEnumerable<object> enumerable)
                        {
                            urlParams.AddRange(enumerable.Select(e => new KeyValuePair<string, object>(param.Key, e)));
                        }
                        else
                        {
                            urlParams.Add(param);
                        }
                    }

                    url += "?" + string.Join("&", urlParams.Select(e => $"{e.Key}={Uri.EscapeDataString(e.Value?.ToString() ?? string.Empty)}"));
                }

                return new Uri(url);
            }
        }

        /// <summary>
        /// Преобразует модель запроса в HTTP запрос
        /// </summary>
        /// <returns></returns>
        public virtual HttpRequestMessage ToHttpRequest()
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod,
                RequestUri = RequestUri,
            };

            return req;
        }
    }
}
