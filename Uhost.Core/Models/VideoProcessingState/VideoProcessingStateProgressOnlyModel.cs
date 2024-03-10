using System.Collections.Generic;
using static Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.VideoProcessingState
{
    public class VideoProcessingStateProgressOnlyModel
    {
        public IDictionary<FileTypes, double> Progresses { get; set; } = new Dictionary<FileTypes, double>();
    }
}
