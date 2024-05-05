using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Role;

namespace Uhost.Core.Models.Role
{
    public class RoleShortViewModel : IEntityLoadable<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CreatedAt { get; set; }

        public int RightCount { get; set; }

        public int UserCount { get; set; }

        public IEnumerable<string> Rights { get; set; }

        public virtual void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CreatedAt = entity.CreatedAt.ToHumanFmt();
            Rights = entity.RoleRights?.Select(e => ((Rights)e.RightId).ToString().ToCamelCase()).ToList();
            RightCount = entity.RoleRights?.Count ?? 0;
            UserCount = entity.UserRoles?.Count ?? 0;
        }
    }
}
