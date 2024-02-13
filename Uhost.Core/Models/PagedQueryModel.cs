using System.ComponentModel.DataAnnotations;
using Uhost.Core.Properties;

namespace Uhost.Core.Models
{
    public abstract class PagedQueryModel : BaseQueryModel
    {
        /// <summary>
        /// Число элементов на одной странице по умолчанию
        /// </summary>
        public const int DefPerPage = 15;

        /// <summary>
        /// Тек. страница
        /// </summary>
        /// <example>1</example>
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_Invalid))]
        public virtual int Page { get; set; } = 1;

        /// <summary>
        /// Число элементов на странице
        /// </summary>
        /// <example>10</example>
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_Invalid))]
        public virtual int PerPage { get; set; } = DefPerPage;
    }
}
