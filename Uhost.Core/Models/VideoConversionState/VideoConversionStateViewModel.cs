using Entity = Uhost.Core.Data.Entities.VideoConversionState;

namespace Uhost.Core.Models.VideoConversionState
{
    public class VideoConversionStateViewModel : VideoConversionStateCreateModel
    {
        public int Id { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Id = entity.Id;
        }
    }
}
