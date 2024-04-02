using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Uhost.Core.Models.Video;
using Entity = Uhost.Core.Data.Entities.Playlist;
using UserEntity = Uhost.Core.Data.Entities.User;
using VideoEntity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Playlist
{
    public class PlaylistViewModel : PlaylistBaseModel
    {
        internal int UserId { get; private set; }

        public UserShortViewModel User { get; set; }

        public VideoShortViewModel FirstVideo => Videos?.FirstOrDefault();

        public ICollection<VideoShortViewModel> Videos { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            UserId = entity.UserId;
            User = entity.User?.ToModel<UserEntity, UserShortViewModel>();
            Videos = entity.PlaylistEntries?
                .OrderBy(e => e.Order)
                .Select(e => e.Video)
                .Where(e => e.DeletedAt == null)
                .ToModelCollection<VideoEntity, VideoShortViewModel>()
                .ToList();
        }
    }
}
