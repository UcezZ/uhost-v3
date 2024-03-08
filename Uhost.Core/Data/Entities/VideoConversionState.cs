using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("VideoConversionStates")]
    public sealed class VideoConversionState : BaseDateTimedEntity
    {
        public enum VideoConversionStates
        {
            Pending,
            Processing,
            Completed,
            Failed
        }

        public int VideoId { get; set; }

        [Required, Column(TypeName = "TEXT")]
        public string Type { get; set; }

        [Required, Column(TypeName = "TEXT")]
        public string State { get; set; }

        public Video Video { get; set; }
    }
}
