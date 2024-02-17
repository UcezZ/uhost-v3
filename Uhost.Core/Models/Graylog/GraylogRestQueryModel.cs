using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Models.RestClient;
using Uhost.Core.Properties;

namespace Uhost.Core.Models.Graylog
{
    public class GraylogRestQueryModel : BaseRestQueryModel
    {
        private const string _queryKey = "query";
        private const string _fieldsKey = "fields";
        private static readonly string _authHeaderValue = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{CoreSettings.GraylogApi?.Login}:{CoreSettings.GraylogApi?.Password}"))}";

        /// <summary>
        /// Запрос
        /// </summary>
        [StringLengthValidation(1, 250, allowEmpty: false)]
        public string Query
        {
            get => UrlParameters.TryGetValue(_queryKey, out string value) ? value : null;
            set => UrlParameters[_queryKey] = value;
        }

        /// <summary>
        /// Поля CSV
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_RequiredFmt))]
        public IEnumerable<string> Fields
        {
            get => UrlParameters.TryGetValue(_queryKey, out string value) ? value.Split(',') : null;
            set => UrlParameters[_fieldsKey] = value?.Join(",");
        }

        protected override HttpMethod HttpMethod => HttpMethod.Get;

        protected override string Url => $"http://{CoreSettings.GraylogApi?.Endpoint}/api/search/universal/relative/export";

        public override HttpRequestMessage ToHttpRequest()
        {
            var req = base.ToHttpRequest();

            req.Headers.Add("Authorization", _authHeaderValue);

            return req;
        }
    }
}
