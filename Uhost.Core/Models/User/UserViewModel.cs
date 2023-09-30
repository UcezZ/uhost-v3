using System.Collections.Generic;
using Uhost.Core.Models.Role;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserViewModel : UserShortViewModel
    {
        public int VideoCount { get; set; }

        public int PlaylistCount { get; set; }

        public IEnumerable<RoleShortViewModel> Roles { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            VideoCount = entity.Videos?.Count ?? default;
        }
    }
}
