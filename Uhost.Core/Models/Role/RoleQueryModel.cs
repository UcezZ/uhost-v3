using System.Collections.Generic;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Role;

namespace Uhost.Core.Models.Role
{
    public class RoleQueryModel : PagedQueryModel
    {
        /// <summary>
        /// ИД роли
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Поле сортировки - <c>Id</c>, <c>Name</c>, <c>CreatedAt</c>
        /// </summary>
        /// <example>Id</example>
        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }

        internal int ExcludedId { get; set; }

        internal string Name { get; set; }

        internal IEnumerable<int> Ids { get; set; }

        internal int UserId { get; set; }

        internal bool IncludeDeleted { get; set; }
    }
}
