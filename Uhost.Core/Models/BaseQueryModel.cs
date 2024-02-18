using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;

namespace Uhost.Core.Models
{
    public abstract class BaseQueryModel
    {
        /// <summary>
        /// Направление сортировки
        /// </summary>
        public enum SortDirections
        {
            /// <summary>
            /// По возрастанию
            /// </summary>
            Asc,

            /// <summary>
            /// По убыванию
            /// </summary>
            Desc
        }

        /// <summary>
        /// Направление сортировки. Допустимые значения: <c>Asc</c>, <c>Desc</c>
        /// </summary>
        /// <example>Asc</example>
        [EnumValidation(typeof(SortDirections), ifNullOrEmpty: SortDirections.Asc, ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortDirection), ErrorMessageResourceType = typeof(CoreStrings))]
        public virtual string SortDirection { get; set; }

        /// <summary>
        /// Поле сортировки
        /// </summary>
        public abstract string SortBy { get; set; }

        /// <summary>
        /// Спаршенное направление сортировки
        /// </summary>
        internal SortDirections? SortDirectParsed => SortDirection.ParseEnum<SortDirections>();

        internal bool IsDescending => SortDirectParsed == SortDirections.Desc;
    }
}
