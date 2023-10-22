using System.Collections.Generic;
using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.Video;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.Video
{
    public class VideoShortViewModel : BaseModel<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CreatedAt { get; set; }

        public string Description { get; set; }

        public string Duration { get; set; }

        public string ThumbnailUrl { get; set; }

        public IEnumerable<string> Resolutions { get; set; }

        public UserShortViewModel User { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CreatedAt = entity.CreatedAt.ToApiFmt();
            Description = entity.Description.Length > 32 ? entity.Description[..32] : entity.Description;
            Duration = entity.Duration.ToHumanFmt();
            User = entity.User?.ToModel<UserEntity, UserShortViewModel>();
        }
    }
}
