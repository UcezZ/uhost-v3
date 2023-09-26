using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Right;

namespace Uhost.Console.Models
{
    public class DefaultRightModel : BaseModel<Entity>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            entity.Id = Id;
            entity.Name = Name;

            return entity;
        }
    }
}
