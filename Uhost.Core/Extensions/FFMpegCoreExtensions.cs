using FFMpegCore;

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
    }
}
