using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("VideoReactions")]
    public class VideoReaction : BaseDateTimedEntity
    {
        public enum VideoReactions
        {
            Like,
            Dislike
        }

        public int UserId { get; set; }

        public int VideoId { get; set; }

        [Required, Column(TypeName = "text")]
        public string Value { get; set; }

        public User User { get; set; }
        public Video Video { get; set; }
    }
}
