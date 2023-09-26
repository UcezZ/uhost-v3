using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Roles")]
    public class Role : BaseDateTimedEntity
    {
        public enum SortBy
        {
            Id,
            Name,
            CreatedAt
        }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        public ICollection<Right> Rights { get; set; }
        public ICollection<RoleRight> RoleRights { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
