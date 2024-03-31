using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Models.Right;
using Uhost.Core.Models.Role;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserViewModel : UserShortViewModel
    {
        public int VideoCount { get; set; }

        public int PlaylistCount { get; set; }

        public IEnumerable<RoleShortViewModel> Roles { get; set; }

        public IEnumerable<RightViewModel> Rights { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            VideoCount = entity.Videos?.Count(e => e.DeletedAt == null) ?? 0;
            PlaylistCount = entity.Playlists?.Count(e => e.DeletedAt == null) ?? 0;
        }
    }
}
