
using static Uhost.Core.Data.Entities.VideoConversionState;

namespace Uhost.Core.Models.VideoConversionState
{
    public class VideoConversionStateQueryModel : PagedQueryModel
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public VideoConversionStates? State { get; set; }

        public string Token { get; set; }

        public bool IncludeDeleted { get; set; }

        public override string SortBy { get; set; }
    }
}
