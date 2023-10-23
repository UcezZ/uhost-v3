using Hardware.Info;
using System;
using System.Linq;

namespace Uhost.Core.Common
{
    public static class HardwareConfig
    {
        private static readonly IHardwareInfo _hwinfo;

        public static string VideoCodec { get; }

        static HardwareConfig()
        {
            _hwinfo = new HardwareInfo();

            _hwinfo.RefreshVideoControllerList();

            VideoCodec = "libx264";

            if (_hwinfo.VideoControllerList.Any(e => e.Manufacturer.Contains("intel", StringComparison.InvariantCultureIgnoreCase)))
            {
                VideoCodec = "h264_qsv";
            }
            if (_hwinfo.VideoControllerList.Any(e => e.Manufacturer.Contains("nvidia", StringComparison.InvariantCultureIgnoreCase)))
            {
                VideoCodec = "h264_nvenc";
            }
        }
    }
}
