using FFMpegCore;
using FFMpegCore.Enums;
using System;
using System.Drawing;
using Uhost.Core.Common;
using static Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Extensions
{
    public static class FFMpegCoreExtensions
    {
        /// <summary>
        /// Задаёт параметр tune
        /// </summary>
        /// <param name="options"></param>
        /// <param name="tune"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithTune(this FFMpegArgumentOptions options, string tune)
        {
            return options.WithCustomArgument($"-tune {tune}");
        }

        /// <summary>
        /// Задаёт параметр preset
        /// </summary>
        /// <param name="options"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithPreset(this FFMpegArgumentOptions options, string preset)
        {
            return options.WithCustomArgument($"-preset {preset}");
        }

        /// <summary>
        /// Задаёт зараметр maxrate
        /// </summary>
        /// <param name="options"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxRate(this FFMpegArgumentOptions options, int rate)
        {
            return options.WithCustomArgument($"-maxrate {rate}k");
        }

        /// <summary>
        /// Получает разрешение видеопотока
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Size GetSize(this VideoStream stream)
        {
            return new Size(stream.Width, stream.Height);
        }

        /// <summary>
        /// Задаёт параметр r
        /// </summary>
        /// <param name="options"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxFramerate(this FFMpegArgumentOptions options, int rate)
        {
            return options.WithCustomArgument($"-r {rate}");
        }

        /// <summary>
        /// Задаёт параметр vsync
        /// </summary>
        /// <param name="options"></param>
        /// <param name="vsync"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithVsync(this FFMpegArgumentOptions options, int vsync)
        {
            if (vsync < 0 || vsync > 2)
            {
                throw new ArgumentOutOfRangeException(nameof(vsync), vsync, "Value must be in range [0..2]");
            }

            return options.WithCustomArgument($"-vsync {vsync}");
        }

        /// <summary>
        /// Задаёт параметр t
        /// </summary>
        /// <param name="options"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxDuration(this FFMpegArgumentOptions options, TimeSpan duration)
        {
            return options.WithCustomArgument($"-t {(int)duration.TotalSeconds}");
        }

        /// <summary>
        /// Применяет заготовку кодирования видео
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mediaInfo"></param>
        /// <param name="type"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions ApplyPreset(this FFMpegArgumentOptions options, IMediaAnalysis mediaInfo, Types type, TimeSpan? duration = null)
        {
            options = options
                .WithVideoCodec(FFConfig.VideoCodec)
                .WithPreset(FFConfig.VideoPresets[Speed.VerySlow])
                .WithTune("hq")
                .WithVsync(2)
                .UsingThreads(Environment.ProcessorCount);

            switch (type)
            {
                case Types.Video240p:
                    options = options
                        .WithAudioBitrate(48)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(240)))
                        .WithVideoBitrate(240)
                        .WithMaxRate(384)
                        .WithMaxFramerate(18);
                    break;
                case Types.Video480p:
                    options = options
                        .WithAudioBitrate(64)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(480)))
                        .WithVideoBitrate(960)
                        .WithMaxRate(1536)
                        .WithMaxFramerate(24);
                    break;
                case Types.Video720p:
                    options = options
                        .WithAudioBitrate(112)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(720)))
                        .WithVideoBitrate(1536)
                        .WithMaxRate(2560)
                        .WithMaxFramerate(48);
                    break;
                case Types.Video1080p:
                    options = options
                        .WithAudioBitrate(144)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(1080)))
                        .WithVideoBitrate(3072)
                        .WithMaxRate(4096)
                        .WithMaxFramerate(60);
                    break;
            }

            if (duration != null)
            {
                options = options.WithMaxDuration((TimeSpan)duration);
            }

            return options;
        }
    }
}
