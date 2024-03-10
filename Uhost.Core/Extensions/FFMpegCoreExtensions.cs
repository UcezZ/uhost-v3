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
        /// Режимы частотности кадроы
        /// </summary>
        public enum FpsMode
        {
            /// <summary>
            /// Постоянная частота кадров
            /// </summary>
            Cfr,

            /// <summary>
            /// Переменная частота кадров
            /// </summary>
            Vfr
        }

        /// <summary>
        /// Форматы пикселей
        /// </summary>
        public enum PixelFormat
        {
            /// <summary>
            /// 16 BPP, 8-8-8
            /// </summary>
            Yuv440p,

            /// <summary>
            /// 12 BPP, 8-8-8
            /// </summary>
            Yuv420p,

            /// <summary>
            /// 8 BPP, 8
            /// </summary>
            Gray,

            /// <summary>
            /// 8 BPP, 8
            /// </summary>
            Pal8,

            /// <summary>
            /// 12 BPP, 8-8-8
            /// </summary>
            Nv12,

            /// <summary>
            /// 16 BPP, 8-8-8
            /// </summary>
            Nv16,

            /// <summary>
            /// 12 BPP, 8-8-8
            /// </summary>
            Nv21
        }

        /// <summary>
        /// Устройства аппаратного ускорения
        /// </summary>
        public enum HardwareAccelerationDevices
        {
            Auto = HardwareAccelerationDevice.Auto,
            D3D11VA = HardwareAccelerationDevice.D3D11VA,
            DXVA2 = HardwareAccelerationDevice.DXVA2,
            QSV = HardwareAccelerationDevice.QSV,
            CUVID = HardwareAccelerationDevice.CUVID,
            VDPAU = HardwareAccelerationDevice.VDPAU,
            VAAPI = HardwareAccelerationDevice.VAAPI,
            LibMFX = HardwareAccelerationDevice.LibMFX,
            CUDA = 8
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
        /// Задаёт параметр <c>-tune</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="tune"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithTune(this FFMpegArgumentOptions options, string tune)
        {
            return options.WithCustomArgument($"-tune {tune}");
        }

        /// <summary>
        /// Задаёт параметр <c>-preset</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithPreset(this FFMpegArgumentOptions options, Speed preset)
        {
            return options.WithCustomArgument($"-preset {FFConfig.VideoPresets[preset]}");
        }

        /// <summary>
        /// Задаёт зараметр <c>-maxrate</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxRate(this FFMpegArgumentOptions options, int rate)
        {
            return options.WithCustomArgument($"-maxrate {rate}k");
        }

        /// <summary>
        /// Задаёт параметр -fpsmax
        /// </summary>
        /// <param name="options"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxFramerate(this FFMpegArgumentOptions options, int rate)
        {
            return options.WithCustomArgument($"-fpsmax {rate}");
        }

        /// <summary>
        /// Задаёт параметр <c>-vsync</c>
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
        /// Задаёт параметр <c>-t</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxDuration(this FFMpegArgumentOptions options, TimeSpan duration)
        {
            return options.WithCustomArgument($"-t {(int)duration.TotalSeconds}");
        }

        /// <summary>
        /// Задаёт параметр <c>-fps_mode</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithFpsMode(this FFMpegArgumentOptions options, FpsMode mode)
        {
            return options.WithCustomArgument($"-fps_mode {mode.ToString().ToLower()}");
        }

        /// <summary>
        /// Задаёт параметр <c>-pix_fmt</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithPixelFormat(this FFMpegArgumentOptions options, PixelFormat format)
        {
            return options.WithCustomArgument($"-pix_fmt {format.ToString().ToLower()}");
        }

        /// <summary>
        /// Задаёт параметр <c>-qmin</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithQMin(this FFMpegArgumentOptions options, byte value)
        {
            return options.WithCustomArgument($"-qmin {value}");
        }

        /// <summary>
        /// Задаёт параметр <c>-qmax</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithQMax(this FFMpegArgumentOptions options, byte value)
        {
            return options.WithCustomArgument($"-qmax {value}");
        }

        /// <summary>
        /// Задаёт параметр <c>-g</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithKeyFrames(this FFMpegArgumentOptions options, byte value)
        {
            return options.WithCustomArgument($"-g {value}");
        }

        /// <summary>
        /// Задаёт параметр <c>-hwaccel</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithHardwareAcceleration(this FFMpegArgumentOptions options, HardwareAccelerationDevices device)
        {
            return options.WithCustomArgument($"-hwaccel {device.ToString().ToLower()}");
        }

        /// <summary>
        /// Задаёт параметр <c>-hwaccel_output_format</c>
        /// </summary>
        /// <param name="options"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithOutputHardwareAcceleration(this FFMpegArgumentOptions options, HardwareAccelerationDevices device)
        {
            return options.WithCustomArgument($"-hwaccel_output_format {device.ToString().ToLower()}");
        }

        /// <summary>
        /// Применяет заготовку кодирования видео
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mediaInfo"></param>
        /// <param name="type"></param>
        /// <param name="maxDuration"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions ApplyOptimalPreset(this FFMpegArgumentOptions options, IMediaAnalysis mediaInfo, FileTypes type, TimeSpan? maxDuration = null)
        {
            options = options
                .WithVideoCodec(FFConfig.VideoCodec)
                .WithPreset(CoreSettings.EncodingSpeed)
                .WithTune("hq")
                .WithFpsMode(FpsMode.Vfr)
                .WithPixelFormat(PixelFormat.Nv12)
                .WithQMin(28)
                .WithQMax(35)
                .WithoutMetadata()
                .WithAudioCodec("aac");

            switch (type)
            {
                case FileTypes.Video240p:
                    options = options
                        .WithAudioBitrate(48)
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(240)))
                        .WithVideoBitrate(240)
                        .WithMaxRate(384)
                        .WithMaxFramerate(18)
                        .WithKeyFrames(30);
                    break;
                case FileTypes.Video480p:
                    options = options
                        .WithAudioBitrate(96)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(480)))
                        .WithVideoBitrate(960)
                        .WithMaxRate(1536)
                        .WithMaxFramerate(24)
                        .WithKeyFrames(30);
                    break;
                case FileTypes.Video720p:
                    options = options
                        .WithAudioBitrate(112)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(720)))
                        .WithVideoBitrate(1536)
                        .WithMaxRate(2560)
                        .WithMaxFramerate(48)
                        .WithKeyFrames(60);
                    break;
                case FileTypes.Video1080p:
                    options = options
                        .WithAudioBitrate(160)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(1080)))
                        .WithVideoBitrate(3840)
                        .WithMaxRate(6144)
                        .WithMaxFramerate(60)
                        .WithKeyFrames(60);
                    break;
            }

            if (maxDuration != null)
            {
                options = options.WithMaxDuration((TimeSpan)maxDuration);
            }

            return options;
        }

        /// <summary>
        /// Преобразует видео в HLS файлы
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="hlsPath"></param>
        /// <param name="addArguments"></param>
        /// <returns></returns>
        public static FFMpegArgumentProcessor OutputToHls(this FFMpegArguments arguments, string hlsPath, Action<FFMpegArgumentOptions> addArguments = null)
        {
            void args(FFMpegArgumentOptions e)
            {
                addArguments?.Invoke(e);
                e.WithCustomArgument("-hls_time 10")
                    .WithCustomArgument("-hls_list_size 0")
                    .WithCustomArgument("-f hls");
            }

            return arguments.OutputToFile(hlsPath, true, args);
        }
    }
}
