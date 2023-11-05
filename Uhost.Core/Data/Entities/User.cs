using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data.Entities
{
    [Table("Users")]
    public class User : BaseDateTimedEntity
    {
        public enum Themes
        {
            Light,
            Dark
        }

        public enum SortBy
        {
            Id,
            CreatedAt,
            LastVisitAt
        }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        [Required, Column(TypeName = "text")]
        public string Desctiption { get; set; }

        [Required, Column(TypeName = "text")]
        public string Login { get; set; }

        [Required, Column(TypeName = "text")]
        public string Email { get; set; }

        [Required, Column(TypeName = "text")]
        public string Password { get; set; }

        [Required, Column(TypeName = "varchar(8)")]
        public string Theme { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? LastVisitAt { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? BlockedAt { get; set; }

        public int? BlockedByUserId { get; set; }

        public string BlockReason { get; set; }

        public User BlockedByUser { get; set; }
        public ICollection<User> BlockedUsers { get; set; }
        public ICollection<Role> Roles { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<File> Files { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<VideoReaction> VideoReactions { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}
