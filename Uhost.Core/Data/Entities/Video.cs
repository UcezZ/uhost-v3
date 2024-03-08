using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Videos")]
    public class Video : BaseDateTimedEntity
    {
        public enum SortBy
        {
            Id,
            Name,
            Duration,
            CreatedAt,
            Random
        }

        public int UserId { get; set; }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        [Required, Column(TypeName = "text")]
        public string Description { get; set; }

        [Required, Column(TypeName = "char(16)")]
        public string Token { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsHidden { get; set; }

        public bool AllowComments { get; set; }

        public bool AllowReactions { get; set; }

        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<VideoReaction> VideoReactions { get; set; }
        public ICollection<PlaylistEntry> PlaylistEntries { get; set; }
        public ICollection<VideoConversionState> VideoConversionStates { get; set; }
    }
}
