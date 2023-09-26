using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("RoleRights")]
    public class RoleRight
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public int RightId { get; set; }
        public Right Right { get; set; }
    }
}
