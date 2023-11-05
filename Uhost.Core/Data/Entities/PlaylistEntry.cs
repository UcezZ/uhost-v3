using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("PlaylistEntries")]
    public class PlaylistEntry : BaseEntity
    {
        public int PlaylistId { get; set; }

        public int VideoId { get; set; }

        public int Order { get; set; }

        public Playlist Playlist { get; set; }

        public Video Video { get; set; }
    }
}
