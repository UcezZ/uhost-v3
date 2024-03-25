using System.Linq;
using static Uhost.Core.Data.Entities.VideoProcessingState;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoShortProcessingModel : IEntityLoadable<Entity>
    {
        internal int Id { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }

        public string ThumbnailUrl { get; set; }

        public string State { get; set; }

        public virtual void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Token = entity.Token;

            if (entity.VideoProcessingStates?.Any() != true)
            {
                State = string.Empty;
            }
            else
            {
                if (entity.VideoProcessingStates.All(e => e.State == nameof(VideoProcessingStates.Completed)))
                {
                    State = nameof(VideoProcessingStates.Completed);
                }
                else if (entity.VideoProcessingStates.Any(e => e.State == nameof(VideoProcessingStates.Failed)))
                {
                    State = nameof(VideoProcessingStates.Failed);
                }
                else if (entity.VideoProcessingStates.Any(e => e.State == nameof(VideoProcessingStates.Processing)))
                {
                    State = nameof(VideoProcessingStates.Processing);
                }
                else
                {
                    State = nameof(VideoProcessingStates.Pending);
                }
            }
        }
    }
}
