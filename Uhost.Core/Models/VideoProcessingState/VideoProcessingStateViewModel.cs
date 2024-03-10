using Entity = Uhost.Core.Data.Entities.VideoProcessingState;

namespace Uhost.Core.Models.VideoProcessingState
{
    public class VideoProcessingStateViewModel : VideoProcessingStateCreateModel
    {
        public int Id { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Id = entity.Id;
        }
    }
}
