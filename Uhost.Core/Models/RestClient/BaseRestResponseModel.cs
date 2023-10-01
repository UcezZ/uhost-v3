using System;
using System.Net.Http;

namespace Uhost.Core.Models.RestClient
{
    /// <summary>
    /// Базовая модель ответа REST API
    /// </summary>
    public class BaseRestResponseModel
    {
        /// <summary>
        /// Признак успешности ответа
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Объект исключения в случае возникновения
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Объект HTTP ответа
        /// </summary>
        public HttpResponseMessage HttpResponse { get; set; }

        /// <summary>
        /// Содержимое ответа в виде строки
        /// </summary>
        public string Contents { get; set; }
    }
}
