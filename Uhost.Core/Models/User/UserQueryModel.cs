using System.Collections.Generic;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserQueryModel : PagedQueryModel
    {
        /// <summary>
        /// ИД пользователя
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Поле сортировки - <c>Id</c>, <c>CreatedAt</c>, <c>LastVisitAt</c>
        /// </summary>
        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortDirection))]
        public override string SortBy { get; set; }
        internal bool IncludeVideos { get; set; }
        internal bool IncludePlaylists { get; set; }
        internal string LoginOrEmail { get; set; }
        internal int ExcludedId { get; set; }
        internal IEnumerable<int> Ids { get; set; }
        internal bool IncludeDeleted { get; set; }
    }
}
