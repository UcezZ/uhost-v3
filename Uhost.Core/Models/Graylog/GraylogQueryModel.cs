using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;

namespace Uhost.Core.Models.Graylog
{
    public class GraylogQueryModel
    {
        /// <summary>
        /// Запрос Graylog
        /// </summary>
        [StringLengthValidation(1, 250, allowEmpty: false)]
        public string Query { get; set; }

        /// <summary>
        /// Возвращаемые поля Graylog
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_RequiredFmt))]
        public IEnumerable<string> Fields { get; set; }

        public static implicit operator GraylogRestQueryModel(GraylogQueryModel obj) =>
            new GraylogRestQueryModel
            {
                Query = obj.Query,
                Fields = obj.Fields
            };
    }
}
