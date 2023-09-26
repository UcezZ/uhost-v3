using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Right;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Role;
using RightEntity = Uhost.Core.Data.Entities.Right;

namespace Uhost.Core.Models.Role
{
    public class RoleViewModel : BaseModel<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<RightViewModel> Rights { get; set; }

        internal IEnumerable<Rights> RightsCasted => Rights?.Select(e => e.RightCasted);

        public override void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Rights = entity.Rights?.ToModelCollection<RightEntity, RightViewModel>() ?? Array.Empty<RightViewModel>();
        }
    }
}
