using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Files")]
    public class File : BaseDateTimedEntity
    {
        public enum Types
        {
            UserAvatar,
            VideoThumbnail,
            VideoRaw,
            Video240p,
            Video360p,
            Video540p,
            Video720p
        }

        public int UserId { get; set; }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        public int Size { get; set; }

        [Required, Column(TypeName = "text")]
        public string Mime { get; set; }

        [Column(TypeName = "varchar(16)")]
        public string Type { get; set; }

        public int? DynId { get; set; }

        [Column(TypeName = "varchar(16)")]
        public string DynName { get; set; }

        [Required, Column(TypeName = "char(40)")]
        public string Digest { get; set; }

        [Required, Column(TypeName = "char(32)")]
        public string Token { get; set; }

        public User User { get; set; }
    }
}
