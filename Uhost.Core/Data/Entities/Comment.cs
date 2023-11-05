using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Comments")]
    public class Comment : BaseDateTimedEntity
    {
        public enum SortBy
        {
            Id,
            CreatedAt,
            UserId,
            VideoId
        }

        public int VideoId { get; set; }

        public int UserId { get; set; }

        [Required, Column(TypeName = "text")]
        public string Text { get; set; }

        public Video Video { get; set; }

        public User User { get; set; }
    }
}
