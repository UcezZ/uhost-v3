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
    public class RoleViewModel : RoleShortViewModel
    {
        public IEnumerable<RightViewModel> Rights { get; set; }

        internal IEnumerable<Rights> RightsCasted => Rights?.Select(e => e.RightCasted);

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Rights = entity.Rights?.ToModelCollection<RightEntity, RightViewModel>() ?? Array.Empty<RightViewModel>();
        }
    }
}
