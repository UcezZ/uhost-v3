using System;
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
            CreatedAt
        }

        public int UserId { get; set; }

        [Required, Column(TypeName = "text")]
        public string Name { get; set; }

        [Required, Column(TypeName = "text")]
        public string Description { get; set; }

        [Required, Column(TypeName = "char(16)")]
        public string Token { get; set; }

        public TimeSpan Duration { get; set; }

        public User User { get; set; }
    }
}
