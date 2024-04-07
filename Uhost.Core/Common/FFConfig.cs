using FFMpegCore.Enums;
using Hardware.Info;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Uhost.Core.Common
{
    public static class FFConfig
    {
        private static readonly IHardwareInfo _hwinfo;

        public static string VideoCodec { get; }

        public static IDictionary<Speed, string> VideoPresets { get; }

        public static string HardwareAcceleration { get; }

        static FFConfig()
        {
            _hwinfo = new HardwareInfo();

            _hwinfo.RefreshVideoControllerList();

            VideoCodec = "libx264";

            VideoPresets = new Dictionary<Speed, string>
            {
                [Speed.VerySlow] = "veryslow",
                [Speed.Slower] = "slower",
                [Speed.Slow] = "slow",
                [Speed.Medium] = "medium",
                [Speed.Fast] = "fast",
                [Speed.Faster] = "faster",
                [Speed.VeryFast] = "veryfast",
                [Speed.SuperFast] = "superfast",
                [Speed.UltraFast] = "ultrafast"
            };

            if (_hwinfo.VideoControllerList.Any(e => e.Manufacturer.Contains("intel", StringComparison.InvariantCultureIgnoreCase)))
            {
                HardwareAcceleration = "qsv";
                VideoCodec = "h264_qsv";
            }
            if (_hwinfo.VideoControllerList.Any(e => e.Manufacturer.Contains("nvidia", StringComparison.InvariantCultureIgnoreCase)))
            {
                HardwareAcceleration = "cuda";
                VideoCodec = "h264_nvenc";
                VideoPresets[Speed.VerySlow] = "p7";
                VideoPresets[Speed.Slower] = "p6";
            }

            foreach (var value in Enum.GetValues<Speed>())
            {
                if (!VideoPresets.ContainsKey(value))
                {
                    VideoPresets[value] = "medium";
                }
            }
        }
    }
}
