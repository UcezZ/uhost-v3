using System.Collections.Generic;
using static Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.VideoConversionState
{
    public class VideoConversionStateProgressOnlyModel
    {
        public IDictionary<FileTypes, double> Progresses { get; set; } = new Dictionary<FileTypes, double>();
    }
}
