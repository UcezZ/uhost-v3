﻿using FFMpegCore;
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
        /// Получает разрешение видеопотока
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Size GetSize(this VideoStream stream)
        {
            return new Size(stream.Width, stream.Height);
        }

        /// <summary>
        /// Задаёт параметр -tune
        /// </summary>
        /// <param name="options"></param>
        /// <param name="tune"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithTune(this FFMpegArgumentOptions options, string tune)
        {
            return options.WithCustomArgument($"-tune {tune}");
        }

        /// <summary>
        /// Задаёт параметр -preset
        /// </summary>
        /// <param name="options"></param>
        /// <param name="preset"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithPreset(this FFMpegArgumentOptions options, Speed preset)
        {
            return options.WithCustomArgument($"-preset {FFConfig.VideoPresets[preset]}");
        }

        /// <summary>
        /// Задаёт зараметр -maxrate
        /// </summary>
        /// <param name="options"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxRate(this FFMpegArgumentOptions options, int rate)
        {
            return options.WithCustomArgument($"-maxrate {rate}k");
        }

        /// <summary>
        /// Задаёт параметр -r
        /// </summary>
        /// <param name="options"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxFramerate(this FFMpegArgumentOptions options, int rate)
        {
            return options.WithCustomArgument($"-r {rate}");
        }

        /// <summary>
        /// Задаёт параметр -vsync
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
        /// Задаёт параметр -t
        /// </summary>
        /// <param name="options"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithMaxDuration(this FFMpegArgumentOptions options, TimeSpan duration)
        {
            return options.WithCustomArgument($"-t {(int)duration.TotalSeconds}");
        }

        /// <summary>
        /// Задаёт параметр -fps_mode
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithFpsMode(this FFMpegArgumentOptions options, FpsMode mode)
        {
            return options.WithCustomArgument($"-fps_mode {mode.ToString().ToLower()}");
        }

        /// <summary>
        /// Задаёт параметр -pix_fmt
        /// </summary>
        /// <param name="options"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions WithPixelFormat(this FFMpegArgumentOptions options, PixelFormat format)
        {
            return options.WithCustomArgument($"-pix_fmt {format.ToString().ToLower()}");
        }

        /// <summary>
        /// Применяет заготовку кодирования видео
        /// </summary>
        /// <param name="options"></param>
        /// <param name="mediaInfo"></param>
        /// <param name="type"></param>
        /// <param name="maxDuration"></param>
        /// <returns></returns>
        public static FFMpegArgumentOptions ApplyPreset(this FFMpegArgumentOptions options, IMediaAnalysis mediaInfo, Types type, TimeSpan? maxDuration = null)
        {
            options = options
                .WithVideoCodec(FFConfig.VideoCodec)
                .WithPreset(Speed.VerySlow)
                .WithTune("hq")
                .WithFpsMode(FpsMode.Vfr)
                .WithPixelFormat(PixelFormat.Nv12)
                .UsingMultithreading(true)
                .WithoutMetadata();

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
                case Types.Video360p:
                    options = options
                        .WithAudioBitrate(64)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(360)))
                        .WithVideoBitrate(640)
                        .WithMaxRate(1024)
                        .WithMaxFramerate(24);
                    break;
                case Types.Video480p:
                    options = options
                        .WithAudioBitrate(96)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(480)))
                        .WithVideoBitrate(1024)
                        .WithMaxRate(1536)
                        .WithMaxFramerate(30);
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
                        .WithAudioBitrate(160)
                        .WithAudioCodec("aac")
                        .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitToMin(1080)))
                        .WithVideoBitrate(3840)
                        .WithMaxRate(6144)
                        .WithMaxFramerate(60);
                    break;
            }

            if (maxDuration != null)
            {
                options = options.WithMaxDuration((TimeSpan)maxDuration);
            }

            return options;
        }
    }
}
