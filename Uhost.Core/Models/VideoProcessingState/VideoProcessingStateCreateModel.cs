using Uhost.Core.Extensions;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.VideoProcessingState;
using Entity = Uhost.Core.Data.Entities.VideoProcessingState;

namespace Uhost.Core.Models.VideoProcessingState
{
    public class VideoProcessingStateCreateModel : BaseModel<Entity>
    {
        public int VideoId { get; set; }

        public FileTypes? Type { get; set; }

        public VideoProcessingStates? State { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            entity.VideoId = VideoId;

            if (Type != null)
            {
                entity.Type = Type.ToString();
            }
            if (State != null)
            {
                entity.State = State.ToString();
            }

            return entity;
        }

        public override void LoadFromEntity(Entity entity)
        {
            VideoId = entity.VideoId;
            Type = entity.Type.ParseEnum<FileTypes>();
            State = entity.State.ParseEnum<VideoProcessingStates>();
        }
    }
}
