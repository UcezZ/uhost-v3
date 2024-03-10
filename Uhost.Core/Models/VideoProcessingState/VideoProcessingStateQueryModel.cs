
using static Uhost.Core.Data.Entities.VideoProcessingState;

namespace Uhost.Core.Models.VideoProcessingState
{
    public class VideoProcessingStateQueryModel : PagedQueryModel
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public VideoProcessingStates? State { get; set; }

        public string Token { get; set; }

        public bool IncludeDeleted { get; set; }

        public override string SortBy { get; set; }
    }
}
