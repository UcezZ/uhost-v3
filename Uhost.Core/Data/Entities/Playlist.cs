using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Playlists")]
    public class Playlist : BaseDateTimedEntity
    {
        public int UserId { get; set; }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        public User User { get; set; }

        public ICollection<PlaylistEntry> PlaylistEntries { get; set; }
    }
}
