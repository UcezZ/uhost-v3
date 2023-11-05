using FFMpegCore;
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
    public sealed class VideoService : BaseService, IVideoService
    {
        private readonly VideoRepository _repo;
        private readonly LogWriter _logger;
        private readonly ISchedulerService _scheduler;
        private readonly IFileService _files;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IRedisDatabase _redis;
        private const string _redisProgressKeyMask = "progress_{0}";
        private static readonly TimeSpan _redisProgressKeyTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan _maxStreamDuration = TimeSpan.FromHours(5);

        private static readonly Types[] _videoResolutionTypes = new[]
        {
            Types.Video240p,
            Types.Video480p,
            Types.Video720p,
            Types.Video1080p
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

        public Entity Add(VideoUploadFileModel model)
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
                _files.Delete(file?.Id ?? 0);
                _repo.SoftDelete(entity.Id);
            }

            return null;
        }

        public Entity Add(VideoUploadUrlModel model)
        {
            if (_contextAccessor?.HttpContext?.User != null && _contextAccessor.HttpContext.User.TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            var entity = _repo.Add(model);

            if (PrepareVideo(entity, model.Url, model.MaxDurationParsed))
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
        /// <param name="url"></param>
        /// <param name="maxDuration"></param>
        /// <returns></returns>
        private bool PrepareVideo(Entity entity, string url, TimeSpan? maxDuration)
        {
            var thumbFile = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"thumb_{Guid.NewGuid()}.jpg")));

            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var mediaInfo = FFProbe.Analyse(new Uri(url));

                    if (mediaInfo.PrimaryVideoStream == null)
                    {
                        return false;
                    }

                    entity.Duration = mediaInfo.Duration;

                    var size = new Size(mediaInfo.PrimaryVideoStream.Width, mediaInfo.PrimaryVideoStream.Height).FitTo(320, 320);

                    var ffargs = FFMpegArguments
                        .FromUrlInput(new Uri(url))
                        .OutputToFile(thumbFile.FullName, true, e => e.WithVideoFilters(vf => vf.Scale(size)).WithFrameOutputCount(1));

                    Tools.MakePath(thumbFile.FullName);

                    if (!ffargs.ProcessSynchronously())
                    {
                        return false;
                    }

                    _files.Add(
                        name: "thumb.jpg",
                        file: thumbFile,
                        type: Types.VideoThumbnail,
                        dynType: typeof(Entity),
                        dynId: entity.Id);

                    _scheduler.ScheduleVideoStreamConvert(entity.Id, Types.Video240p, url, maxDuration ?? _maxStreamDuration);

                    if (mediaInfo.PrimaryVideoStream.Height >= 480)
                    {
                        _scheduler.ScheduleVideoStreamConvert(entity.Id, Types.Video480p, url, maxDuration ?? _maxStreamDuration);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 720)
                    {
                        _scheduler.ScheduleVideoStreamConvert(entity.Id, Types.Video720p, url, maxDuration ?? _maxStreamDuration);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 1080)
                    {
                        _scheduler.ScheduleVideoStreamConvert(entity.Id, Types.Video1080p, url, maxDuration ?? _maxStreamDuration);
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
                    var mediaInfo = FFProbe.Analyse(rawVideoPath);

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
                        name: "thumb.jpg",
                        file: thumbFile,
                        type: Types.VideoThumbnail,
                        dynType: typeof(Entity),
                        dynId: entity.Id);

                    _scheduler.ScheduleVideoConvert(entity.Id, Types.Video240p);

                    if (mediaInfo.PrimaryVideoStream.Height >= 480)
                    {
                        _scheduler.ScheduleVideoConvert(entity.Id, Types.Video480p);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 720)
                    {
                        _scheduler.ScheduleVideoConvert(entity.Id, Types.Video720p);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 1080)
                    {
                        _scheduler.ScheduleVideoConvert(entity.Id, Types.Video1080p);
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

        /// <summary>
        /// Конвертация загруженного файла видео
        /// </summary>
        /// <param name="id">ИД сущности видео</param>
        /// <param name="type">Тип видео</param>
        /// <returns></returns>
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
            var mediaInfo = await FFProbe.AnalyseAsync(file.Path);
            var ffargs = FFMpegArguments
                .FromFileInput(file.Path)
                .OutputToFile(output.FullName, true, e => e.ApplyPreset(mediaInfo, type));

            Tools.MakePath(output.FullName);

            await _logger.WriteLineAsync($"Processing video #{id} ({type}) with arguments:\r\n\r\n{ffargs?.Arguments}\r\n", LogWriter.Severity.Info);

            try
            {
                var result = await ffargs?
                    .NotifyOnProgress(async e => await OnProgress(e, id, type), mediaInfo.Duration)
                    .ProcessAsynchronously();

                if (result == true && output.Length > 0)
                {
                    _files.Add(
                        output,
                        name: "video.mp4",
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
            }
            catch (Exception e)
            {
                e.Data["args"] = ffargs.Arguments;
                throw;
            }

            await OnProgress(100, id, type);

            await _logger.WriteLineAsync($"Processing video #{id} ({type}) completed", LogWriter.Severity.Info);

            try
            {
                output.Delete();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }

        /// <summary>
        /// Конвертация видео из потока
        /// </summary>
        /// <param name="id">ИД сущности видео</param>
        /// <param name="type">Тип видео</param>
        /// <param name="url">URL потока</param>
        /// <param name="maxDuration">Максимальная продолжительность</param>
        /// <returns></returns>
        public async Task ConvertUrl(int id, Types type, string url, TimeSpan maxDuration)
        {
            if (!_videoResolutionTypes.Contains(type))
            {
                throw new ArgumentException("Wrong file type specified", nameof(type));
            }

            var output = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"temp_{Guid.NewGuid()}.mp4")));
            var mediaInfo = await FFProbe.AnalyseAsync(new Uri(url));
            var ffargs = FFMpegArguments
                .FromUrlInput(new Uri(url))
                .OutputToFile(output.FullName, true, e => e.ApplyPreset(mediaInfo, type, maxDuration));

            Tools.MakePath(output.FullName);

            await _logger.WriteLineAsync($"Processing video #{id} ({type}) with arguments:\r\n\r\n{ffargs?.Arguments}\r\n", LogWriter.Severity.Info);

            try
            {
                var result = await ffargs?
                    .NotifyOnProgress(async e => await OnProgress(e, id, type), mediaInfo.Duration)
                    .ProcessAsynchronously();

                if (result == true && output.Length > 0)
                {
                    _files.Add(
                        output,
                        name: "video.mp4",
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
            }
            catch (Exception e)
            {
                e.Data["args"] = ffargs.Arguments;
                throw;
            }

            await OnProgress(100, id, type);

            await _logger.WriteLineAsync($"Processing video #{id} ({type}) completed", LogWriter.Severity.Info);

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
