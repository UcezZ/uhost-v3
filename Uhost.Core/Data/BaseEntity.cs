using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uhost.Core.Data
{
    /// <summary>
    /// Base entity class 
    /// </summary>
    public abstract class BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), ScaffoldColumn(false)]
        public int Id { get; set; }
    }
}
