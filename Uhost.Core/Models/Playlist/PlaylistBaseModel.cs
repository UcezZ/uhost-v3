using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.Playlist;

namespace Uhost.Core.Models.Playlist
{
    public class PlaylistBaseModel : IEntityLoadable<Entity>, IEntityFillable<Entity>
    {
        public string Name { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Name = Name?.TrimAll() ?? string.Empty;

            return entity;
        }

        public virtual void LoadFromEntity(Entity entity)
        {
            Name = entity.Name;
        }
    }
}
