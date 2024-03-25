using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Right;

namespace Uhost.Console.Models
{
    public class DefaultRightModel : IEntityFillable<Entity>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Id = Id;
            entity.Name = Name;

            return entity;
        }
    }
}
