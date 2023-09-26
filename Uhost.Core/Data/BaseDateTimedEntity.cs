using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data
{
    /// <summary>
    /// Base entity class with timestamps
    /// </summary>
    public abstract class BaseDateTimedEntity : BaseEntity
    {
        [Column(TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? UpdatedAt { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? DeletedAt { get; set; }
    }
}
