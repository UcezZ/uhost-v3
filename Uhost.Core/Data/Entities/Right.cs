using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Rights")]
    public class Right : BaseEntity
    {
        public enum Rights
        {
            VideoCreateUpdate = 1
        }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        public ICollection<Role> Roles { get; set; }
        public ICollection<RoleRight> RoleRights { get; set; }
    }
}
