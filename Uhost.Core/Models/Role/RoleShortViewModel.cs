using Entity = Uhost.Core.Data.Entities.Role;

namespace Uhost.Core.Models.Role
{
    public class RoleShortViewModel : BaseModel<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
        }
    }
}
