using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Rights")]
    public class Right : BaseEntity
    {
        public enum Rights : int
        {
            VideoCreateUpdate = 0x01,
            VideoDelete = 0x02,
            VideoGetAll = 0x03,

            FileGet = 0x11,
            FileCreateUpdate = 0x12,
            FileDelete = 0x13,

            UserCreate = 0x21,
            UserDelete = 0x22,
            UserInteractAll = 0x23,

            AdminLogAccess = 0x31,
            AdminSessionAccess = 0x32,
            AdminSessionTerminate = 0x33,

            RoleCreateUpdate = 0x41,
            RoleDelete = 0x42
        }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        public ICollection<Role> Roles { get; set; }
        public ICollection<RoleRight> RoleRights { get; set; }
    }
}
