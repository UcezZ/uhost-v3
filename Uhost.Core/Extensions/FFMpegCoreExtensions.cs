using FFMpegCore;
using System.Drawing;

namespace Uhost.Core.Extensions
{
    public static class FFMpegCoreExtensions
    {
        public static FFMpegArgumentOptions WithTune(this FFMpegArgumentOptions options, string tune)
        {
            return options.WithCustomArgument($"-tune {tune}");
        }
        public static FFMpegArgumentOptions WithPreset(this FFMpegArgumentOptions options, string preset)
        {
            return options.WithCustomArgument($"-preset {preset}");
        }
        public static FFMpegArgumentOptions WithMaxRate(this FFMpegArgumentOptions options, int rate)
        {
            return options.WithCustomArgument($"-maxrate {rate}k");
        }
        public static Size GetSize(this VideoStream stream)
        {
            return new Size(stream.Width, stream.Height);
        }
    }
}
