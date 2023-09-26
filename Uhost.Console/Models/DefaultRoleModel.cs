using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.Role;
using RoleRightEntity = Uhost.Core.Data.Entities.RoleRight;

namespace Uhost.Console.Models
{
    public class DefaultRoleModel : BaseModel<Entity>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<int> RightIds { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            entity.Id = Id;
            entity.Name = Name;
            entity.RoleRights ??= new List<RoleRightEntity>();

            if (RightIds != null)
            {
                entity.RoleRights.AddRangeDistinctBy(RightIds.Select(e => new RoleRightEntity { RightId = e }), e => e.RightId);
            }

            return entity;
        }
    }
}
