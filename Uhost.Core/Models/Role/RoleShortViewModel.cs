using Entity = Uhost.Core.Data.Entities.Role;

namespace Uhost.Core.Models.Role
{
    public class RoleShortViewModel : IEntityLoadable<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
        }
    }
}
