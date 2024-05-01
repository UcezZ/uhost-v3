using FFMpegCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
using Uhost.Core.Models;
using Uhost.Core.Models.File;
using Uhost.Core.Models.Video;
using Uhost.Core.Models.VideoProcessingState;
using Uhost.Core.Repositories;
using Uhost.Core.Services.Comment;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Log;
using Uhost.Core.Services.RedisSwitcher;
using Uhost.Core.Services.Scheduler;
using Uhost.Core.Services.Token;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Log;
using static Uhost.Core.Data.Entities.Right;
using static Uhost.Core.Data.Entities.VideoProcessingState;
using Entity = Uhost.Core.Data.Entities.Video;
using FileEntity = Uhost.Core.Data.Entities.File;
using QueryModel = Uhost.Core.Models.Video.VideoQueryModel;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Services.Video
{
    public sealed class VideoService : BaseService, IVideoService
    {
        private readonly VideoRepository _repo;
        private readonly VideoProcessingStateRepository _processingStates;
        private readonly LogWriter _logger;
        private readonly ISchedulerService _scheduler;
        private readonly IFileService _fileService;
        private readonly IRedisSwitcherService _redis;
        private readonly ITokenService _tokens;
        private readonly ICommentService _commentService;
        private readonly ILogService _log;
        private const string _redisProgressKeyMask = "progress_{0}";
        private static readonly TimeSpan _redisProgressKeyTtl = TimeSpan.FromMinutes(5);
        private static readonly TimeSpan _maxStreamDuration = TimeSpan.FromHours(4);

        private static readonly FileTypes[] _videoFileTypes = new[]
        {
            FileTypes.Video240p,
            FileTypes.Video480p,
            FileTypes.Video720p,
            FileTypes.Video1080p,
            FileTypes.VideoWebm
        };

        public static IReadOnlyCollection<FileTypes> VideoFileTypes => _videoFileTypes;

        public VideoService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IServiceProvider provider,
            ISchedulerService scheduler,
            IFileService fileService,
            ICommentService commentService,
            IRedisSwitcherService redis,
            ITokenService tokens,
            ILogService log) : base(factory, provider)
        {
            _repo = new VideoRepository(_dbContext);
            _processingStates = new VideoProcessingStateRepository(_dbContext);
            _logger = provider.GetService<LogWriter>();
            _scheduler = scheduler;
            _fileService = fileService;
            _redis = redis;
            _commentService = commentService;
            _tokens = tokens;
            _log = log;
        }

        /// <summary>
        /// Получение всех видео по запросу
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public PagerResultModel<VideoShortViewModel> GetAllPaged(QueryModel query)
        {
            var pager = _repo
                .GetAll<VideoShortViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var files = _fileService
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.Id), typeof(Entity))
                    .ToList();

                var userAvatars = _fileService
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.UserId), typeof(UserEntity), FileTypes.UserAvatar)
                    .ToList();

                foreach (var model in pager)
                {
                    model.ThumbnailUrl = files
                        .FirstOrDefault(e => e.DynId == model.Id && e.TypeParsed == FileTypes.VideoThumbnail)?
                        .Url;
                    model.Resolutions = files
                        .Where(e => e.DynId == model.Id && _videoFileTypes.Contains(e.TypeParsed))
                        .Select(e => e.Type.ToCamelCase())
                        .OrderBy(e => e.ParseDigits());

                    if (model.User != null)
                    {
                        model.User.AvatarUrl = userAvatars.FirstOrDefault(e => e.DynId == model.UserId)?.Url;
                    }
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
                Page = 1,
                PerPage = count,
                SortBy = nameof(Entity.SortBy.Random)
            };

            OverrideByUserRestrictions(query);

            return GetAllPaged(query).Items;
        }

        /// <summary>
        /// Получение одного видео по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VideoViewModel GetOne(int id)
        {
            var query = new QueryModel
            {
                Id = id,
                ForceShowForUser = true
            };

            OverrideByUserRestrictions(query);

            var model = _repo
                .GetAll<VideoViewModel>(query)
                .FirstOrDefault();

            return FillViewModel(model);
        }

        /// <summary>
        /// Создаёт ключи redis для проверуки токена
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task CreateRedisKeys(VideoViewModel model)
        {
            if (model == null)
            {
                return;
            }

            var keyTtl = model.DurationObj.Add(TimeSpan.FromHours(24));

            TryGetUserIp(out var ip);

            foreach (var url in model.UrlPaths.Values)
            {
                var value = new
                {
                    model.Token,
                    model.AccessToken,
                    Url = url.Replace("/master.m3u8", string.Empty, StringComparison.InvariantCultureIgnoreCase),
                    Ip = ip?.ToString()
                };

                var keyPayload = $"{value.AccessToken}{value.Url}{CoreSettings.VideoTokenSalt}";
                var keyHash = keyPayload.ComputeHash(HasherExtensions.EncryptionMethod.MD5);
                var key = $"videotoken_{keyHash}";

                await _tokens.StoreVideoKeyAsync(key, value, keyTtl);
            }
        }

        /// <summary>
        /// Получение одного видео по токену
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<VideoViewModel> GetOne(string token)
        {
            var query = new QueryModel
            {
                Token = token,
                ForceShowForUser = true
            };

            OverrideByUserRestrictions(query);

            var model = _repo
                .GetAll<VideoViewModel>(query)
                .FirstOrDefault();

            if (model != null)
            {
                FillViewModel(model);
                await CreateRedisKeys(model);
            }

            return model;
        }

        /// <summary>
        /// Заполнение модели видео
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private VideoViewModel FillViewModel(VideoViewModel model)
        {
            if (model == null)
            {
                return null;
            }

            var files = _fileService
                .GetByDynEntity<FileShortViewModel>(model.Id, typeof(Entity))
                .ToList();

            if (model.User != null)
            {
                model.User.AvatarUrl = _fileService
                    .GetByDynEntity<FileShortViewModel>(model.UserId, typeof(UserEntity), FileTypes.UserAvatar)
                    .FirstOrDefault()?.Url;
            }

            model.ThumbnailUrl = files
                .FirstOrDefault(e => e.DynId == model.Id && e.TypeParsed == FileTypes.VideoThumbnail)?
                .Url;
            var videoFiles = files
                .Where(e => _videoFileTypes.Contains(e.TypeParsed))
                .OrderBy(e => e.Type.ParseDigits())
                .ToList();
            model.Resolutions = videoFiles
                .Select(e => e.Type.ToCamelCase())
                .OrderBy(e => e.ParseDigits());
            model.UrlPaths = videoFiles.ToDictionary(e => e.Type, e => e.UrlPath);

            var h264files = videoFiles.Where(e => e.TypeParsed != FileTypes.VideoWebm);

            if (h264files.Any())
            {
                model.UrlPaths["Hls"] = Tools.UrlCombine(
                    CoreSettings.HlsUrl,
                    $",{h264files.Select(e => e.UrlPath).Join(",")},.urlset",
                    "master.m3u8");
            }

            model.Urls = model.UrlPaths.ToDictionary(e => e.Key, e => Tools.UrlCombine(CoreSettings.MediaServerUrl, e.Value));
            model.DownloadSizes = videoFiles.ToDictionary(e => e.Type, e => e.Size.ToHumanSize());

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
            var file = _fileService.Add(model.File, FileTypes.VideoRaw, typeof(Entity), entity.Id);

            if (file != null && PrepareVideo(entity, file))
            {
                _repo.Save();
                _log.Add(Events.VideoUploaded, new
                {
                    entity.Id,
                    entity.Token,
                    Model = model.ToPropertiesDictionary(e => e.Name != nameof(model.File)),
                    File = new
                    {
                        model.File.FileName,
                        model.File.ContentType,
                        model.File.Length
                    }
                });

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
        public Entity Add(VideoUploadUrlModel model)
        {
            if (TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            var entity = _repo.Add(model);

            if (PrepareVideo(entity, model.Url, model.MaxDurationParsed))
            {
                _repo.Save();
                _log.Add(Events.VideoUploaded, new
                {
                    entity.Id,
                    entity.Token,
                    Model = model
                });

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
            _log.Add(Events.VideoEdited, new
            {
                Token = token,
                Model = model
            });
        }

        /// <summary>
        /// Удаление видео
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            _repo.SoftDelete(id);
            _log.Add(Events.VideoDeleted, new
            {
                Id = id
            });
        }

        /// <summary>
        /// Удаление видео
        /// </summary>
        /// <param name="token"></param>
        public void Delete(string token)
        {
            _repo.Perform(e => e.DeletedAt = DateTime.Now, e => e.Token == token && e.DeletedAt == null);
            _log.Add(Events.VideoDeleted, new
            {
                Token = token
            });
        }

        /// <summary>
        /// Планирует скачивание видео
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="url"></param>
        private void EnqueueFetch(int videoId, string url)
        {
            var model = new VideoProcessingStateCreateModel
            {
                VideoId = videoId,
                State = VideoProcessingStates.Pending,
                Type = FileTypes.VideoRaw
            };
            var processingState = _processingStates.Add(model);

            _scheduler.ScheduleVideoStreamFetch(processingState.Id, url);
        }

        /// <summary>
        /// Подготавливает видео из URL, генерирует картинку, добавляет задачи на конвертацию
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
                if (string.IsNullOrEmpty(url))
                {
                    return false;
                }

                var mediaInfo = FFProbe.Analyse(new Uri(url));

                if (mediaInfo.PrimaryVideoStream == null)
                {
                    return false;
                }

                entity.Duration = mediaInfo.Duration.TotalSeconds < 1
                    ? maxDuration ?? _maxStreamDuration
                    : mediaInfo.Duration;

                var size = new Size(mediaInfo.PrimaryVideoStream.Width, mediaInfo.PrimaryVideoStream.Height).FitTo(320, 320);

                var ffargs = FFMpegArguments
                    .FromUrlInput(new Uri(url))
                    .OutputToFile(thumbFile.FullName, true, e => e.WithVideoFilters(vf => vf.Scale(size)).WithFrameOutputCount(1));

                Tools.EnsurePathToFileExist(thumbFile.FullName);

                if (!ffargs.ProcessSynchronously())
                {
                    return false;
                }

                _fileService.Add(
                    name: "thumb.jpg",
                    file: thumbFile,
                    type: FileTypes.VideoThumbnail,
                    dynType: typeof(Entity),
                    dynId: entity.Id);

                EnqueueFetch(entity.Id, url);

                return true;
            }
            finally
            {
                thumbFile.TryDeleteIfExists();
            }
        }

        /// <summary>
        /// Планирует конвертирование видео
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="type"></param>
        private void EnqueueConversion(int videoId, FileTypes type)
        {
            var model = new VideoProcessingStateCreateModel
            {
                VideoId = videoId,
                Type = type,
                State = VideoProcessingStates.Pending
            };
            var processingState = _processingStates.Add(model);

            _scheduler.ScheduleVideoConvert(processingState.Id);
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

                var capTime = TimeSpan.FromSeconds(mediaInfo.Duration.TotalSeconds * 0.1);

                var size = new Size(mediaInfo.PrimaryVideoStream.Width, mediaInfo.PrimaryVideoStream.Height);

                if (FFMpegCoreAdditions.TryGetSize(rawVideoPath, out var altSize))
                {
                    size = altSize;
                }

                var thumbSize = size.FitTo(320, 320);

                var ffargs = FFMpegArguments
                    .FromFileInput(rawVideoPath)
                    .OutputToFile(thumbFile.FullName, true, e => e.WithVideoFilters(vf => vf.Scale(thumbSize)).Seek(capTime).WithFrameOutputCount(1));

                Tools.EnsurePathToFileExist(thumbFile.FullName);

                if (!ffargs.ProcessSynchronously())
                {
                    return false;
                }

                _fileService.Add(
                    name: "thumb.jpg",
                    file: thumbFile,
                    type: FileTypes.VideoThumbnail,
                    dynType: typeof(Entity),
                    dynId: entity.Id);

                EnqueueConversion(entity.Id, FileTypes.Video240p);
                EnqueueConversion(entity.Id, FileTypes.VideoWebm);

                if (size.Height >= 480)
                {
                    EnqueueConversion(entity.Id, FileTypes.Video480p);
                }
                if (size.Height >= 720)
                {
                    EnqueueConversion(entity.Id, FileTypes.Video720p);
                }
                if (size.Height >= 1080)
                {
                    EnqueueConversion(entity.Id, FileTypes.Video1080p);
                }

                return true;
            }
            finally
            {
                thumbFile.TryDeleteIfExists();
            }
        }

        /// <summary>
        /// Задача конвертации загруженного файла видео
        /// </summary>
        /// <param name="processingStateId">ИД сущности статуса видео</param>
        /// <returns></returns>
        public async Task Convert(int processingStateId)
        {
            var processingQuery = new VideoProcessingStateQueryModel
            {
                Id = processingStateId
            };
            var processingState = _processingStates
                .GetAll<VideoProcessingStateViewModel>(processingQuery)
                .FirstOrDefault();

            if (!_videoFileTypes.Any(e => e == processingState?.Type))
            {
                var exception = new Exception("Wrong file type specified");
                exception.Data["State"] = processingState;
                SentrySdk.CaptureException(exception);
                throw exception;
            }

            var file = _fileService
                .GetByDynEntity<FileShortViewModel>(processingState.VideoId, typeof(Entity), FileTypes.VideoRaw)
                .FirstOrDefault();

            if (file == null || !file.Exists)
            {
                if (!file.Exists)
                {
                    _logger?.WriteLine($"File \"{file.Path}\" not found, convetring #{processingState.VideoId}, {processingState.Type}", LogWriter.Severity.Warn);
                    SentrySdk.CaptureMessage($"File \"{file.Path}\" not found, convetring #{processingState.VideoId}, {processingState.Type}", SentryLevel.Warning);
                }

                return;
            }

            var extension = processingState.Type == FileTypes.VideoWebm ? "webm" : "mp4";
            var output = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"temp_{Guid.NewGuid()}.{extension}")));
            var mediaInfo = await FFProbe.AnalyseAsync(file.Path);
            var ffargs = FFMpegArguments
                .FromFileInput(file.Path, true, e => e.WithHardwareAcceleration(CoreSettings.InputHardwareAcceleration))
                .OutputToFile(output.FullName, true, e => e.ApplyOptimalPreset(mediaInfo, (FileTypes)processingState.Type));

            await DoConversion(ffargs, output, processingState);

            if (_processingStates.AreAllCompleted(processingState.VideoId))
            {
                _fileService.DeleteByDynParams(processingState.VideoId, typeof(Entity), FileTypes.VideoRaw, true);
            }
        }

        /// <summary>
        /// Задача конвертации видео из потока
        /// </summary>
        /// <param name="processingStateId">ИД сущности статуса видео</param>
        /// <param name="url">URL потока</param>
        /// <returns></returns>
        public async Task FetchStream(int processingStateId, string url)
        {
            var processingQuery = new VideoProcessingStateQueryModel
            {
                Id = processingStateId
            };
            var processingState = _processingStates.GetAll<VideoProcessingStateViewModel>(processingQuery)
                .FirstOrDefault();

            if (processingState?.Type != FileTypes.VideoRaw)
            {
                var exception = new Exception("Wrong file type specified");
                exception.Data["State"] = processingState;
                SentrySdk.CaptureException(exception);
                throw exception;
            }

            _processingStates.UpdateState(processingStateId, VideoProcessingStates.Processing);

            var extension = processingState.Type == FileTypes.VideoWebm ? "webm" : "mp4";
            var output = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"temp_{Guid.NewGuid()}.{extension}")));
            Tools.EnsurePathToFileExist(output.FullName);
            _repo.GetTokenAndDuration(processingState.VideoId, out var token, out var duration);
            var ffargs = FFMpegArguments
                .FromUrlInput(new Uri(url))
                .OutputToFile(output.FullName, true, e => e
                    .WithVideoCodec("copy")
                    .WithAudioCodec("copy")
                    .WithMaxDuration(duration));

            await _logger?.WriteLineAsync($"Fetching video #{processingState.VideoId},{token} with arguments:\r\n{ffargs?.Arguments}", LogWriter.Severity.Info);
            Exception ffException = null;

            try
            {
                await ffargs?
                    .NotifyOnProgress(async e => await OnFFProgressAsync(e, token, FileTypes.VideoRaw), duration)
                    .ProcessAsynchronously();

                _log.Add(Events.VideoFetchCompleted, new
                {
                    ProcessingStateId = processingStateId,
                    ProcessingState = processingState,
                    ffargs.Arguments
                });
            }
            catch (Exception e)
            {
                _processingStates.UpdateState(processingStateId, VideoProcessingStates.Failed);
                await _logger?.WriteLineAsync($"An error occured while fetching video #{processingState.VideoId},{token} with arguments:\r\n{ffargs?.Arguments}", LogWriter.Severity.Warn);
                e.Data[nameof(ffargs.Arguments)] = ffargs.Arguments;
                SentrySdk.CaptureException(e);
                ffException = e;

                _log.Add(Events.VideoFetchFailed, new
                {
                    ProcessingStateId = processingStateId,
                    ProcessingState = processingState,
                    ffargs.Arguments
                });
            }

            if (output.Exists && output.Length > 0)
            {
                var file = _fileService.Add(
                     output,
                     name: "raw.mp4",
                     type: FileTypes.VideoRaw,
                     dynType: typeof(Entity),
                     dynId: processingState.VideoId);

                await OnFFProgressAsync(100, token, FileTypes.VideoRaw);
                await _logger?.WriteLineAsync($"Fetching video #{processingState.VideoId},{token} completed", LogWriter.Severity.Info);

                var mediaInfo = await FFProbe.AnalyseAsync(file.GetPath());

                if (mediaInfo.Duration.TotalSeconds > 0)
                {
                    _repo.UpdateDuration(processingState.VideoId, mediaInfo.Duration);
                }

                EnqueueConversion(processingState.VideoId, FileTypes.Video240p);
                EnqueueConversion(processingState.VideoId, FileTypes.VideoWebm);

                if (mediaInfo.PrimaryVideoStream.Height >= 480)
                {
                    EnqueueConversion(processingState.VideoId, FileTypes.Video480p);
                }
                if (mediaInfo.PrimaryVideoStream.Height >= 720)
                {
                    EnqueueConversion(processingState.VideoId, FileTypes.Video720p);
                }
                if (mediaInfo.PrimaryVideoStream.Height >= 1080)
                {
                    EnqueueConversion(processingState.VideoId, FileTypes.Video1080p);
                }

                _processingStates.UpdateState(processingStateId, VideoProcessingStates.Completed);
            }
            else
            {
                var exception = new Exception("Failed to fetch video", ffException);
                exception.Data["State"] = processingState;
                exception.Data[nameof(ffargs.Arguments)] = ffargs?.Arguments;
                SentrySdk.CaptureException(exception);
            }

            output.TryDeleteIfExists();
        }

        /// <summary>
        /// Общий метод конвертации
        /// </summary>
        /// <param name="ffargs"></param>
        /// <param name="output"></param>
        /// <param name="processingState"></param>
        /// <returns></returns>
        private async Task DoConversion(FFMpegArgumentProcessor ffargs, FileInfo output, VideoProcessingStateViewModel processingState)
        {
            Tools.EnsurePathToFileExist(output.FullName);
            _repo.GetTokenAndDuration(processingState.VideoId, out var token, out var duration);

            await _logger?.WriteLineAsync($"Processing video #{processingState.VideoId},{token} ({processingState.Type}) with arguments:\r\n{ffargs?.Arguments}", LogWriter.Severity.Info);

            try
            {
                _processingStates.UpdateState(processingState.Id, VideoProcessingStates.Processing);

                var extension = processingState.Type == FileTypes.VideoWebm ? "webm" : "mp4";
                var result = await ffargs?
                    .NotifyOnProgress(async e => await OnFFProgressAsync(e, token, processingState.Type.Value), duration)
                    .ProcessAsynchronously();

                if (result == true && output.Length > 0)
                {
                    _fileService.Add(
                        output,
                        name: $"video.{extension}",
                        type: processingState.Type,
                        dynType: typeof(Entity),
                        dynId: processingState.VideoId);
                }
                else
                {
                    var exception = new Exception("Failed to process video");
                    exception.Data["State"] = processingState;
                    exception.Data["Arguments"] = ffargs?.Arguments;
                    throw exception;
                }

                await OnFFProgressAsync(100, token, processingState.Type.Value);
                await _logger?.WriteLineAsync($"Processing video #{processingState.VideoId},{token} ({processingState.Type}) completed", LogWriter.Severity.Info);
                _processingStates.UpdateState(processingState.Id, VideoProcessingStates.Completed);

                _log.Add(Events.VideoConversionCompleted, new
                {
                    ProcessingState = processingState,
                    ffargs.Arguments
                });
            }
            catch (Exception e)
            {
                e.Data["State"] = processingState;
                e.Data["Arguments"] = ffargs?.Arguments;
                SentrySdk.CaptureException(e);
                _processingStates.UpdateState(processingState.Id, VideoProcessingStates.Failed);

                _log.Add(Events.VideoConversionFailed, new
                {
                    ProcessingState = processingState,
                    ffargs.Arguments
                });

                throw;
            }
            finally
            {
                output.TryDeleteIfExists();
            }
        }

        /// <summary>
        /// Событие прогресса конвертации
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task OnFFProgressAsync(double progress, string token, FileTypes type)
        {
            try
            {
                var key = _redisProgressKeyMask.Format(token);
                var value = await _redis.ExecuteAsync(async e => await e.StringGetAsync(key));
                var model = !value.IsNullOrEmpty && value.TryCastTo<VideoProcessingStateProgressOnlyModel>(out var casted) ? casted : new VideoProcessingStateProgressOnlyModel();
                model.Progresses[type] = progress;
                await _redis.ExecuteAsync(async e => await e.StringSetAsync(key, model.ToJson(), expiry: _redisProgressKeyTtl, flags: CommandFlags.FireAndForget));
            }
            catch { }
        }

        /// <summary>
        /// Получает прогресс конвертации видео
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<VideoProcessingStateProgressModel> GetConversionProgressAsync(string token)
        {
            var model = _processingStates.GetProgresses(token);
            var key = _redisProgressKeyMask.Format(token);
            var value = await _redis.ExecuteAsync(async e => await e.StringGetAsync(key));

            if (!value.IsNullOrEmpty && value.TryCastTo<VideoProcessingStateProgressOnlyModel>(out var casted))
            {
                model.LoadFrom(casted);
            }

            return model;
        }

        /// <summary>
        /// Переопределение параметров запроса в соответствии с правами пользователя
        /// </summary>
        /// <param name="query"></param>
        public void OverrideByUserRestrictions(QueryModel query)
        {
            if (TryGetUserRights(out var rights) && TryGetUserId(out var userId))
            {
                query.ShowHidden &= rights.Contains(Rights.VideoGetAll) || query.UserId == userId;
                query.ShowPrivate &= rights.Contains(Rights.VideoGetAll) || query.UserId == userId;

                if (query.ForceShowForUser)
                {
                    query.ShowHiddenForUserId = userId;
                    query.ShowPrivateForUserId = userId;
                }
            }
            else
            {
                query.ShowHidden = false;
                query.ShowPrivate = false;
            }

            if (!string.IsNullOrEmpty(query.Token))
            {
                query.ShowHidden = true;
            }
        }

        /// <summary>
        /// Получает ссылку на загрузку и имя файла
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="stream"></param>
        /// <param name="lastModified"></param>
        /// <returns></returns>
        public bool TryGetDownload(string token, FileTypes type, out string name, out Stream stream, out DateTime lastModified)
        {
            name = default;
            stream = default;
            lastModified = default;

            var query = new QueryModel
            {
                Token = token
            };
            OverrideByUserRestrictions(query);

            var video = _repo.PrepareQuery(query)
                .Select(e => new { e.Id, e.Name, LastModified = e.UpdatedAt ?? e.CreatedAt })
                .FirstOrDefault();

            if (video == null)
            {
                return false;
            }

            var extension = type == FileTypes.VideoWebm ? "webm" : "mp4";
            lastModified = video.LastModified;
            name = $"{video.Name}.{extension}";

            var file = _fileService.GetByDynEntity<FileShortViewModel>(video.Id, typeof(Entity), type).FirstOrDefault();

            if (file == null || !file.Exists)
            {
                return false;
            }

            try
            {
                stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
                return true;
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                return false;
            }
        }

        public PagerResultModel<VideoShortProcessingModel> GetAllProcessingsPaged(QueryModel query)
        {
            // это ограничение нужно только здесь
            if (TryGetUserRights(out var rights) && TryGetUserId(out var userId))
            {
                if (!rights.Contains(Rights.VideoGetAll))
                {
                    query.UserId = userId;
                    query.ShowHiddenForUserId = userId;
                    query.ShowPrivateForUserId = userId;
                }
                else
                {
                    query.ShowHidden = true;
                    query.ShowPrivate = true;
                }
            }

            query.IncludeProcessingStates = true;

            var pager = _repo
                .GetAll<VideoShortProcessingModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var files = _fileService
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.Id), typeof(Entity), FileTypes.VideoThumbnail)
                    .ToList();

                foreach (var model in pager)
                {
                    model.ThumbnailUrl = files.FirstOrDefault(e => e.DynId == model.Id)?.Url ?? string.Empty;
                }
            }

            return pager.Paginate();
        }
    }
}
