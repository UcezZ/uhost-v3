using FFMpegCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Sentry;
using Sentry.Protocol;
using StackExchange.Redis;
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
using Uhost.Core.Services.Comment;
using Uhost.Core.Services.File;
using Uhost.Core.Services.RedisSwitcher;
using Uhost.Core.Services.Scheduler;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Right;
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
        private readonly IFileService _fileService;
        private readonly IRedisSwitcherService _redis;
        private readonly ICommentService _commentService;
        private const string _redisProgressKeyMask = "progress_{0}";
        private static readonly TimeSpan _redisProgressKeyTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan _maxStreamDuration = TimeSpan.FromHours(4);

        private static readonly Types[] _videoFileTypes = new[]
        {
            Types.Video240p,
            Types.Video480p,
            Types.Video360p,
            Types.Video720p,
            Types.Video1080p
        };

        public static IReadOnlyCollection<Types> VideoFileTypes => _videoFileTypes;

        public VideoService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IServiceProvider provider,
            ISchedulerService scheduler,
            IFileService fileService,
            ICommentService commentService,
            IRedisSwitcherService redis) : base(factory, provider)
        {
            _repo = new VideoRepository(_dbContext);
            _logger = provider.GetService<LogWriter>();
            _scheduler = scheduler;
            _fileService = fileService;
            _redis = redis;
            _commentService = commentService;
        }

        /// <summary>
        /// Получение всех видео по запросу
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public object GetAllPaged(QueryModel query)
        {
            var pager = _repo
                .GetAll<VideoShortViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var files = _fileService
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.Id), typeof(Entity))
                    .ToList();

                foreach (var model in pager)
                {
                    model.ThumbnailUrl = files
                        .FirstOrDefault(e => e.DynId == model.Id && e.TypeParsed == Types.VideoThumbnail)?
                        .Url;
                    model.Resolutions = files
                        .Where(e => e.DynId == model.Id && _videoFileTypes.Contains(e.TypeParsed))
                        .Select(e => e.Type.ParseDigits())
                        .Where(e => e > 0)
                        .OrderBy(e => e)
                        .Select(e => $"{e}p");
                }
            }

            return pager.Paginate();
        }

        /// <summary>
        /// Получение случайных видео
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<VideoShortViewModel> GetRandom(int count)
        {
            var query = new QueryModel
            {
                SortBy = nameof(Entity.SortBy.Random)
            };

            OverrideByUserRestrictions(query);

            var models = _repo
                .GetAll<VideoShortViewModel>(query)
                .Take(count)
                .ToList();

            if (models.Any())
            {
                var files = _fileService
                    .GetByDynEntity<FileShortViewModel>(models.Select(e => e.Id), typeof(Entity))
                    .ToList();

                foreach (var model in models)
                {
                    model.ThumbnailUrl = files
                        .FirstOrDefault(e => e.DynId == model.Id && e.TypeParsed == Types.VideoThumbnail)?
                        .Url;
                    model.Resolutions = files
                        .Where(e => e.DynId == model.Id && _videoFileTypes.Contains(e.TypeParsed))
                        .Select(e => e.Type.ParseDigits())
                        .Where(e => e > 0)
                        .OrderBy(e => e)
                        .Select(e => $"{e}p");
                }
            }

            return models;
        }

        /// <summary>
        /// Получение одного видео по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VideoViewModel GetOne(int id)
        {
            var query = new QueryModel { Id = id };

            OverrideByUserRestrictions(query);

            var model = _repo
                .GetAll<VideoViewModel>(query)
                .FirstOrDefault();

            return FillViewModel(model);
        }

        private async Task CreateRedisKeys(VideoViewModel model)
        {
            if (!TryGetUserIp(out var ip))
            {
                return;
            }

            model.CookieTtl = model.DurationObj.Add(TimeSpan.FromHours(1));

            foreach (var url in model.UrlPaths.Values)
            {
                var value = new
                {
                    model.Token,
                    model.AccessToken,
                    Url = url,
                    Ip = ip.ToString()
                };

                var keyPayload = LocalEnvironment.IsDev
                    ? $"{value.AccessToken}{value.Url}{CoreSettings.VideoTokenSalt}"
                    : $"{value.AccessToken}{value.Url}{value.Ip}{CoreSettings.VideoTokenSalt}";
                var keyHash = keyPayload.ComputeHash(HasherExtensions.EncryptionMethod.MD5);
                var key = $"videotoken_{keyHash}";

                await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, value.ToJson(Formatting.Indented), model.CookieTtl));
            }
        }

        /// <summary>
        /// Получение одного видео по токену
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<VideoViewModel> GetOne(string token)
        {
            var query = new QueryModel { Token = token };

            OverrideByUserRestrictions(query);

            var model = _repo
                .GetAll<VideoViewModel>(query)
                .FirstOrDefault();

            FillViewModel(model);
            await CreateRedisKeys(model);

            return model;
        }

        /// <summary>
        /// Заполнение модели видео
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private VideoViewModel FillViewModel(VideoViewModel model)
        {
            if (model != null)
            {
                var files = _fileService
                    .GetByDynEntity<FileShortViewModel>(model.Id, typeof(Entity))
                    .ToList();

                model.ThumbnailUrl = files
                    .FirstOrDefault(e => e.DynId == model.Id && e.TypeParsed == Types.VideoThumbnail)?
                    .Url;
                var videoFiles = files
                    .Where(e => e.DynId == model.Id && _videoFileTypes.Contains(e.TypeParsed));
                model.Resolutions = videoFiles
                    .Select(e => e.Type.ParseDigits())
                    .Where(e => e > 0)
                    .OrderBy(e => e)
                    .Select(e => $"{e}p");
                model.UrlPaths = videoFiles.ToDictionary(e => e.Type, e => e.UrlPath);

                if (videoFiles.Any())
                {
                    model.UrlPaths["Hls"] = Tools.UrlCombine(
                        CoreSettings.HlsUrl,
                        $",{videoFiles.Select(e => e.UrlPath).Join(",")},.urlset",
                        "master.m3u8");
                }

                model.Urls = model.UrlPaths.ToDictionary(e => e.Key, e => Tools.UrlCombine(CoreSettings.MediaServerUrl, e.Value));
            }

            return model;
        }

        /// <summary>
        /// Загрузка видео из файла
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Entity Add(VideoUploadFileModel model)
        {
            if (TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            var entity = _repo.Add(model);
            var file = _fileService.Add(model.File, Types.VideoRaw, typeof(Entity), entity.Id);

            if (file != null && PrepareVideo(entity, file))
            {
                _repo.Save();

                return entity;
            }
            else
            {
                _fileService.Delete(file?.Id ?? 0);
                _repo.SoftDelete(entity.Id);
            }

            return null;
        }

        /// <summary>
        /// Загрузка видео из URL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <param name="isInfinite"></param>
        public Entity Add(VideoUploadUrlModel model, out bool isInfinite)
        {
            if (TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            var entity = _repo.Add(model);

            if (PrepareVideo(entity, model.Url, model.MaxDurationParsed, out isInfinite))
            {
                _repo.Save();

                return entity;
            }
            else
            {
                _repo.SoftDelete(entity.Id);

                return null;
            }
        }

        /// <summary>
        /// Обновление видео
        /// </summary>
        /// <param name="token"></param>
        /// <param name="model"></param>
        public void Update(string token, VideoUpdateModel model)
        {
            _repo.Update(e => e.Token == token, model);
        }

        /// <summary>
        /// Удаление видео
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            _repo.SoftDelete(id);
        }

        /// <summary>
        /// Удаление видео
        /// </summary>
        /// <param name="token"></param>
        public void Delete(string token)
        {
            _repo.Perform(e => e.DeletedAt = DateTime.Now, e => e.Token == token && e.DeletedAt == null);
        }

        /// <summary>
        /// Подготавливает видео из URL, генерирует картинку, добавляет задачи на конвертацию
        /// </summary>
        /// <param name="entity">Сущность видео</param>
        /// <param name="url"></param>
        /// <param name="maxDuration"></param>
        /// <param name="isInfinite"></param>
        /// <returns></returns>
        private bool PrepareVideo(Entity entity, string url, TimeSpan? maxDuration, out bool isInfinite)
        {
            var thumbFile = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"thumb_{Guid.NewGuid()}.jpg")));

            isInfinite = false;

            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return false;
                }

                var mediaInfo = FFProbe.Analyse(new Uri(url));

                if (mediaInfo.PrimaryVideoStream == null)
                {
                    return false;
                }

                isInfinite = mediaInfo.Duration.TotalSeconds < 1;

                entity.Duration = isInfinite ? maxDuration ?? _maxStreamDuration : mediaInfo.Duration;

                var size = new Size(mediaInfo.PrimaryVideoStream.Width, mediaInfo.PrimaryVideoStream.Height).FitTo(320, 320);

                var ffargs = FFMpegArguments
                    .FromUrlInput(new Uri(url))
                    .OutputToFile(thumbFile.FullName, true, e => e.WithVideoFilters(vf => vf.Scale(size)).WithFrameOutputCount(1));

                Tools.MakePath(thumbFile.FullName);

                if (!ffargs.ProcessSynchronously())
                {
                    return false;
                }

                _fileService.Add(
                    name: "thumb.jpg",
                    file: thumbFile,
                    type: Types.VideoThumbnail,
                    dynType: typeof(Entity),
                    dynId: entity.Id);

                _scheduler.ScheduleVideoStreamFetch(entity.Id, url);

                return true;
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
        /// Подготавливает видео, вычисляет продолжительность и генерирует картинку, добавляет задачи на конвертацию
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

                if (string.IsNullOrEmpty(rawVideoPath))
                {
                    return false;
                }

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

                _fileService.Add(
                    name: "thumb.jpg",
                    file: thumbFile,
                    type: Types.VideoThumbnail,
                    dynType: typeof(Entity),
                    dynId: entity.Id);

                _scheduler.ScheduleVideoConvert(entity.Id, Types.Video240p);

                if (mediaInfo.PrimaryVideoStream.Height >= 360)
                {
                    _scheduler.ScheduleVideoConvert(entity.Id, Types.Video360p);
                }
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
        /// Задача конвертации загруженного файла видео
        /// </summary>
        /// <param name="id">ИД сущности видео</param>
        /// <param name="type">Тип видео</param>
        /// <returns></returns>
        public async Task Convert(int id, Types type)
        {
            if (!_videoFileTypes.Contains(type))
            {
                throw new ArgumentException("Wrong file type specified", nameof(type));
            }

            var file = _fileService
                .GetByDynEntity<FileShortViewModel>(id, typeof(Entity), Types.VideoRaw)
                .FirstOrDefault();

            if (file == null || !file.Exists)
            {
                if (!file.Exists)
                {
                    _logger?.WriteLine($"File \"{file.Path}\" not found, convetring #{id}, {type}", LogWriter.Severity.Warn);
                    SentrySdk.CaptureMessage($"File \"{file.Path}\" not found, convetring #{id}, {type}", SentryLevel.Warning);
                }

                return;
            }

            var output = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"temp_{Guid.NewGuid()}.mp4")));
            var mediaInfo = await FFProbe.AnalyseAsync(file.Path);
            var ffargs = FFMpegArguments
                .FromFileInput(file.Path)
                .OutputToFile(output.FullName, true, e => e.ApplyOptimalPreset(mediaInfo, type));

            await DoConversion(ffargs, output, id, type);
        }

        /// <summary>
        /// Задача конвертации видео из потока
        /// </summary>
        /// <param name="id">ИД сущности видео</param>
        /// <param name="url">URL потока</param>
        /// <returns></returns>
        public async Task FetchStream(int id, string url)
        {
            var output = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"temp_{Guid.NewGuid()}.mp4")));
            Tools.MakePath(output.FullName);
            var duration = _repo.GetDuration(id);
            var token = _repo.GetToken(id);
            var ffargs = FFMpegArguments
                .FromUrlInput(new Uri(url))
                .OutputToFile(output.FullName, true, e => e
                    .WithVideoCodec("copy")
                    .WithAudioCodec("copy")
                    .WithMaxDuration(duration));

            await _logger?.WriteLineAsync($"Fetching video #{id},{token} with arguments:\r\n{ffargs?.Arguments}", LogWriter.Severity.Info);

            try
            {
                var result = await ffargs?
                    .NotifyOnProgress(async e => await OnFetchProgressAsync(e, token), duration)
                    .ProcessAsynchronously();

                if (result == true && output.Length > 0)
                {
                    var file = _fileService.Add(
                         output,
                         name: "raw.mp4",
                         type: Types.VideoRaw,
                         dynType: typeof(Entity),
                         dynId: id);

                    var mediaInfo = await FFProbe.AnalyseAsync(file.GetPath());

                    _scheduler.ScheduleVideoConvert(id, Types.Video240p);

                    if (mediaInfo.PrimaryVideoStream.Height >= 360)
                    {
                        _scheduler.ScheduleVideoConvert(id, Types.Video360p);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 480)
                    {
                        _scheduler.ScheduleVideoConvert(id, Types.Video480p);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 720)
                    {
                        _scheduler.ScheduleVideoConvert(id, Types.Video720p);
                    }
                    if (mediaInfo.PrimaryVideoStream.Height >= 1080)
                    {
                        _scheduler.ScheduleVideoConvert(id, Types.Video1080p);
                    }
                }
                else
                {
                    var exception = new Exception("Failed to fetch video");
                    exception.Data["Id"] = id;
                    exception.Data["Arguments"] = ffargs?.Arguments;

                    SentrySdk.CaptureException(exception);
                }
            }
            catch (Exception e)
            {
                e.Data["args"] = ffargs.Arguments;
                throw;
            }

            await OnFetchProgressAsync(100, token);

            await _logger?.WriteLineAsync($"Fetching video #{id},{token} completed", LogWriter.Severity.Info);

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
        /// Общий метод конвертации
        /// </summary>
        /// <param name="ffargs"></param>
        /// <param name="output"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task DoConversion(FFMpegArgumentProcessor ffargs, FileInfo output, int id, Types type)
        {
            Tools.MakePath(output.FullName);
            var token = _repo.GetToken(id);
            var duration = _repo.GetDuration(id);

            await _logger?.WriteLineAsync($"Processing video #{id},{token} ({type}) with arguments:\r\n{ffargs?.Arguments}", LogWriter.Severity.Info);

            try
            {
                var result = await ffargs?
                    .NotifyOnProgress(async e => await OnConversionProgressAsync(e, token, type), duration)
                    .ProcessAsynchronously();

                if (result == true && output.Length > 0)
                {
                    _fileService.Add(
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

            await OnConversionProgressAsync(100, token, type);

            await _logger?.WriteLineAsync($"Processing video #{id},{token} ({type}) completed", LogWriter.Severity.Info);

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
        /// Событие прогресса копирования URL
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task OnFetchProgressAsync(double progress, string token)
        {
            try
            {
                var key = _redisProgressKeyMask.Format(token);
                var value = await _redis.ExecuteAsync(async e => await e.StringGetAsync(key));
                var model = !value.IsNullOrEmpty && value.TryCastTo<VideoConversionProgressModel>(out var casted) ? casted : new VideoConversionProgressModel();
                model.Fetch = progress;
                await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, model.ToJson(), expiry: _redisProgressKeyTtl, flags: CommandFlags.FireAndForget));
            }
            catch { }
        }

        /// <summary>
        /// Событие прогресса конвертации
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task OnConversionProgressAsync(double progress, string token, Types type)
        {
            try
            {
                var key = _redisProgressKeyMask.Format(token);
                var value = await _redis.ExecuteAsync(async e => await e.StringGetAsync(key));
                var model = !value.IsNullOrEmpty && value.TryCastTo<VideoConversionProgressModel>(out var casted) ? casted : new VideoConversionProgressModel();
                model.Resolutions[type] = progress;
                await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, model.ToJson(), expiry: _redisProgressKeyTtl, flags: CommandFlags.FireAndForget));
            }
            catch { }
        }

        /// <summary>
        /// Получает прогресс конвертации видео
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<VideoConversionProgressModel> GetConversionProgressAsync(string token)
        {
            var key = _redisProgressKeyMask.Format(token);
            var value = await _redis.ExecuteAsync(async e => await e.StringGetAsync(key));
            var model = !value.IsNullOrEmpty && value.TryCastTo<VideoConversionProgressModel>(out var casted) ? casted : null;

            return model;
        }

        public void OverrideByUserRestrictions(QueryModel query)
        {
            if (TryGetUserRights(out var rights) && TryGetUserId(out var userId))
            {
                query.ShowHidden &= rights.Contains(Rights.VideoGetAll) || query.UserId == userId;
                query.ShowPrivate &= rights.Contains(Rights.VideoGetAll) || query.UserId == userId;
            }
            else
            {
                query.ShowHidden = false;
                query.ShowPrivate = false;
            }
        }
    }
}
