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
            VideoCreateUpdate = 0x01,
            VideoDelete = 0x02,
            VideoGetAll = 0x03,
            FileGet = 0x11,
            FileCreateUpdate = 0x12,
            FileDelete = 0x13
        }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        public ICollection<Role> Roles { get; set; }
        public ICollection<RoleRight> RoleRights { get; set; }
    }
}
