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

        public enum Locales
        {
            Ru,
            En
        }

        public enum SortBy
        {
            Id,
            CreatedAt,
            LastVisitAt
        }

        [Required, Column(TypeName = "VARCHAR")]
        public string Name { get; set; }

        [Required, Column(TypeName = "VARCHAR")]
        public string Desctiption { get; set; }

        [Required, Column(TypeName = "VARCHAR")]
        public string Login { get; set; }

        [Required, Column(TypeName = "VARCHAR")]
        public string Email { get; set; }

        [Required, Column(TypeName = "TEXT")]
        public string Password { get; set; }

        [Required, Column(TypeName = "VARCHAR(8)")]
        public string Theme { get; set; }

        [Required, Column(TypeName = "VARCHAR(8)")]
        public string Locale { get; set; }

        [Column(TypeName = "TIMESTAMP")]
        public DateTime? LastVisitAt { get; set; }

        [Column(TypeName = "TIMESTAMP")]
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
        public ICollection<Reaction> Reactions { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}
