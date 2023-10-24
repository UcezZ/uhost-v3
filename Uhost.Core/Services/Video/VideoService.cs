using FFMpegCore;
using FFMpegCore.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Sentry.Protocol;
using StackExchange.Redis;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uhost.Core.Common;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.File;
using Uhost.Core.Models.Video;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Scheduler;
using static Uhost.Core.Data.Entities.File;
using Entity = Uhost.Core.Data.Entities.Video;
using FileEntity = Uhost.Core.Data.Entities.File;
using QueryModel = Uhost.Core.Models.Video.VideoQueryModel;

namespace Uhost.Core.Services.Video
{
    public class VideoService : BaseService, IVideoService
    {
        private readonly VideoRepository _repo;
        private readonly LogWriter _logger;
        private readonly ISchedulerService _scheduler;
        private readonly IFileService _files;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRedisDatabase _redis;
        private const string _redisProgressKeyMask = "progress_{0}";
        private static readonly TimeSpan _redisProgressKeyTtl = TimeSpan.FromMinutes(5);

        private static readonly Types[] _videoResolutionTypes = new[]
        {
            Types.Video240p,
            Types.Video360p,
            Types.Video540p,
            Types.Video720p
        };

        public VideoService(PostgreSqlDbContext context, IServiceProvider provider, ISchedulerService scheduler, IFileService files, IRedisDatabase redis) : base(context)
        {
            _repo = new VideoRepository(_dbContext);
            _contextAccessor = provider.GetService<IHttpContextAccessor>();
            _logger = provider.GetService<LogWriter>();
            _scheduler = scheduler;
            _files = files;
            _redis = redis;
        }

        public object GetAllPaged(QueryModel query)
        {
            var pager = _repo
                .GetAll<VideoShortViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var files = _files
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.Id), typeof(Entity))
                    .ToList();

                foreach (var model in pager)
                {
                    model.ThumbnailUrl = files
                        .FirstOrDefault(e => e.DynId == model.Id && e.TypeParsed == Types.VideoThumbnail)?
                        .Url;
                    model.Resolutions = files
                        .Where(e => e.DynId == model.Id && _videoResolutionTypes.Contains(e.TypeParsed))
                        .Select(e => e.Type[5..]);
                }
            }

            return pager.Paginate();
        }

        public VideoViewModel GetOne(int id)
        {
            var model = _repo
                .GetAll<VideoViewModel>(new QueryModel { Id = id })
                .FirstOrDefault();

            if (model != null)
            {
                var files = _files
                    .GetByDynEntity<FileShortViewModel>(model.Id, typeof(Entity))
                    .ToList();

                model.ThumbnailUrl = files
                    .FirstOrDefault(e => e.DynId == model.Id && e.TypeParsed == Types.VideoThumbnail)?
                    .Url;
                var videoFiles = files
                    .Where(e => e.DynId == model.Id && _videoResolutionTypes.Contains(e.TypeParsed));
                model.Resolutions = videoFiles.Select(e => e.Type[5..]);
                model.Urls = videoFiles.ToDictionary(e => e.TypeParsed, e => e.Url);
            }

            return model;
        }

        public Entity Add(VideoCreateModel model)
        {
            if (_contextAccessor?.HttpContext?.User != null && _contextAccessor.HttpContext.User.TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            var entity = _repo.Add(model);
            var file = _files.Add(model.File, Types.VideoRaw, typeof(Entity), entity.Id);

            if (file != null && PrepareVideo(entity, file))
            {
                _repo.Save();

                return entity;
            }
            else
            {
                _repo.SoftDelete(entity.Id);
            }

            return null;
        }

        public void Delete(int id)
        {
            _repo.SoftDelete(id);
        }

        /// <summary>
        /// Подготавливает видео, вычисляет продолжительность и генерирует картинку
        /// </summary>
        /// <param name="entity">Сущность видео</param>
        /// <param name="rawVideo">Сущность файла загруженного видео</param>
        /// <returns></returns>
        private bool PrepareVideo(Entity entity, FileEntity rawVideo)
        {
            var thumbFile = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"thumb_{Guid.NewGuid()}.jpg")));

            try
            {
                var rawVideoPath = rawVideo.GetPath();
                if (!string.IsNullOrEmpty(rawVideoPath))
                {
                    var mediaInfo = FFProbe.Analyse(rawVideo.GetPath());

                    if (mediaInfo.PrimaryVideoStream == null)
                    {
                        return false;
                    }

                    entity.Duration = mediaInfo.Duration;

                    var capTime = TimeSpan.FromSeconds(mediaInfo.Duration.TotalSeconds * 0.05);
                    var size = new Size(mediaInfo.PrimaryVideoStream.Width, mediaInfo.PrimaryVideoStream.Height).FitTo(320, 320);

                    var ffargs = FFMpegArguments
                        .FromFileInput(rawVideoPath)
                        .OutputToFile(thumbFile.FullName, true, e => e.WithVideoFilters(vf => vf.Scale(size)).Seek(capTime).WithFrameOutputCount(1));

                    Tools.MakePath(thumbFile.FullName);

                    if (!ffargs.ProcessSynchronously())
                    {
                        return false;
                    }

                    _files.Add(
                        file: thumbFile,
                        type: Types.VideoThumbnail,
                        dynType: typeof(Entity),
                        dynId: entity.Id);

                    _scheduler.ScheduleVideoConvert(entity.Id, Types.Video240p);

                    if (mediaInfo.PrimaryVideoStream.Height >= 360)
                    {
                        _scheduler.ScheduleVideoConvert(entity.Id, Types.Video360p);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 540)
                    {
                        _scheduler.ScheduleVideoConvert(entity.Id, Types.Video540p);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 720)
                    {
                        _scheduler.ScheduleVideoConvert(entity.Id, Types.Video720p);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);

                throw;
            }
            finally
            {
                try
                {
                    if (thumbFile.Exists)
                    {
                        thumbFile.Delete();
                    }
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                }
            }
        }

        public async Task Convert(int id, Types type)
        {
            if (!_videoResolutionTypes.Contains(type))
            {
                throw new ArgumentException("Wrong file type specified", nameof(type));
            }

            var file = _files
                .GetByDynEntity<FileShortViewModel>(id, typeof(Entity), Types.VideoRaw)
                .FirstOrDefault();

            if (file == null || !file.Exists)
            {
                if (!file.Exists)
                {
                    SentrySdk.CaptureMessage($"File \"{file.Path}\" not found", SentryLevel.Warning);
                }

                return;
            }

            var output = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"temp_{Guid.NewGuid()}.mp4")));
            var mediaInfo = FFProbe.Analyse(file.Path);
            FFMpegArgumentProcessor ffargs = null;

            switch (type)
            {
                case Types.Video240p:
                    ffargs = FFMpegArguments
                        .FromFileInput(file.Path)
                        .OutputToFile(output.FullName, true, e => e
                            .WithAudioBitrate(48)
                            .WithAudioCodec("aac")
                            .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitTo(height: 240)))
                            .WithVideoCodec(FFConfig.VideoCodec)
                            .WithVideoBitrate(240)
                            .WithMaxRate(384)
                            .WithPreset(FFConfig.VideoPresets[Speed.VerySlow])
                            .WithTune("hq")
                            .WithMaxFramerate(18)
                            .WithVsync(2)
                            .UsingThreads(Environment.ProcessorCount)
                        );
                    break;
                case Types.Video360p:
                    ffargs = FFMpegArguments
                        .FromFileInput(file.Path)
                        .OutputToFile(output.FullName, true, e => e
                            .WithAudioBitrate(64)
                            .WithAudioCodec("aac")
                            .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitTo(height: 360)))
                            .WithVideoCodec(FFConfig.VideoCodec)
                            .WithVideoBitrate(480)
                            .WithMaxRate(768)
                            .WithPreset(FFConfig.VideoPresets[Speed.VerySlow])
                            .WithTune("hq")
                            .WithMaxFramerate(24)
                            .WithVsync(2)
                            .UsingThreads(Environment.ProcessorCount)
                        );
                    break;
                case Types.Video540p:
                    ffargs = FFMpegArguments
                        .FromFileInput(file.Path)
                        .OutputToFile(output.FullName, true, e => e
                            .WithAudioBitrate(96)
                            .WithAudioCodec("aac")
                            .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitTo(height: 540)))
                            .WithVideoCodec(FFConfig.VideoCodec)
                            .WithVideoBitrate(1024)
                            .WithMaxRate(1536)
                            .WithPreset(FFConfig.VideoPresets[Speed.VerySlow])
                            .WithTune("hq")
                            .WithMaxFramerate(30)
                            .WithVsync(2)
                            .UsingThreads(Environment.ProcessorCount)
                        );
                    break;
                case Types.Video720p:
                    ffargs = FFMpegArguments
                        .FromFileInput(file.Path)
                        .OutputToFile(output.FullName, true, e => e
                            .WithAudioBitrate(128)
                            .WithAudioCodec("aac")
                            .WithVideoFilters(vf => vf.Scale(mediaInfo.PrimaryVideoStream.GetSize().FitTo(height: 720)))
                            .WithVideoCodec(FFConfig.VideoCodec)
                            .WithVideoBitrate(1536)
                            .WithMaxRate(2560)
                            .WithPreset(FFConfig.VideoPresets[Speed.VerySlow])
                            .WithTune("hq")
                            .WithMaxFramerate(48)
                            .WithVsync(2)
                            .UsingThreads(Environment.ProcessorCount)
                        );
                    break;
            }

            Tools.MakePath(output.FullName);

            await _logger.WriteLineAsync($"Processing video #{id} ({type}) with arguments:\r\n\r\n{ffargs?.Arguments}\r\n", LogWriter.Severity.Info);

            var result = await ffargs?
                .NotifyOnProgress(async e => await OnProgress(e, id, type), mediaInfo.Duration)
                .ProcessAsynchronously();

            await OnProgress(100, id, type);

            await _logger.WriteLineAsync($"Processing video #{id} ({type}) completed", LogWriter.Severity.Info);

            if (result == true && output.Length > 0)
            {
                _files.Add(
                    output,
                    type: type,
                    dynType: typeof(Entity),
                    dynId: id);
            }
            else
            {
                var exception = new Exception("Failed to process video");
                exception.Data["Id"] = id;
                exception.Data["Type"] = type;
                exception.Data["Arguments"] = ffargs?.Arguments;

                SentrySdk.CaptureException(exception);
            }

            try
            {
                output.Delete();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        private async Task OnProgress(double progress, int videoId, Types type)
        {
            var key = _redisProgressKeyMask.Format(videoId);
            var value = await _redis.Database.StringGetAsync(key);
            var dict = !value.IsNullOrEmpty && value.TryCastTo<IDictionary<Types, double>>(out var casted) ? casted : new Dictionary<Types, double>();
            dict[type] = progress;
            await _redis.Database.StringSetAsync(key, dict.ToJson(), expiry: _redisProgressKeyTtl, flags: CommandFlags.FireAndForget);
        }

        public async Task<IDictionary<Types, double>> GetConversionProgress(int videoId)
        {
            var key = _redisProgressKeyMask.Format(videoId);
            var value = await _redis.Database.StringGetAsync(key);
            var dict = !value.IsNullOrEmpty && value.TryCastTo<IDictionary<Types, double>>(out var casted) ? casted : new Dictionary<Types, double>();

            return dict;
        }
    }
}
