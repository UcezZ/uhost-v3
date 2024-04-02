using Entity = Uhost.Core.Data.Entities.Playlist;

namespace Uhost.Core.Models.Playlist
{
    public class PlaylistCreateModel : PlaylistUpdateModel
    {
        internal int UserId { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.UserId = UserId;

            return entity;
        }
    }
}
