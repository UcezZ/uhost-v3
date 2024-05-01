using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;

namespace Uhost.Core.Models.Session
{
    public sealed class SessionQueryModel : PagedQueryModel
    {
        public enum SessionSortBy
        {
            UserId,
            ExpiresIn
        }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Поле сортировки - <c>UserId</c>, <c>ExpiresIn</c>
        /// </summary>
        /// <example>UserId</example>
        [EnumValidation(typeof(SessionSortBy), nameof(SessionSortBy.UserId), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }
    }
}
