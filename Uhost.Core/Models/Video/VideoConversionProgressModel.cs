using System.Collections.Generic;
using static Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.Video
{
    public sealed class VideoConversionProgressModel
    {
        public double Fetch { get; set; } = 100.0;
        public IDictionary<Types, double> Resolutions { get; set; } = new Dictionary<Types, double>();
    }
}
