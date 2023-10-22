using FFMpegCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private readonly ISchedulerService _scheduler;
        private readonly IFileService _files;
        private readonly IHttpContextAccessor _contextAccessor;
        private static readonly Types[] _videoResolutionTypes = new[]
        {
            Types.Video240p,
            Types.Video360p,
            Types.Video540p,
            Types.Video720p
        };

        public VideoService(PostgreSqlDbContext context, IServiceProvider provider, ISchedulerService scheduler, IFileService files) : base(context)
        {
            _repo = new VideoRepository(_dbContext);
            _contextAccessor = provider.GetService<IHttpContextAccessor>();
            _scheduler = scheduler;
            _files = files;
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

        public void Convert(int id, int typeId)
        {
            Convert(id, (Types)typeId);
        }

        public void Convert(int id, Types type)
        {
            if (!_videoResolutionTypes.Contains(type))
            {
                throw new ArgumentException("Wrong file type specified", nameof(type));
            }

            var file = _files
                .GetByDynEntity<FileShortViewModel>(id, typeof(Entity), Types.VideoRaw)
                .FirstOrDefault();

            if (file == null)
            {
                return;
            }

            var tempFile = new FileInfo(Path.GetFullPath(Path.Combine(Path.GetFullPath("tmp"), $"temp_{Guid.NewGuid()}.mp4")));
            var mediaInfo = FFProbe.Analyse(file.Path);
            FFMpegArgumentProcessor ffargs = null;

            switch (type)
            {
                case Types.Video240p:
                    ffargs = FFMpegArguments
                        .FromFileInput(file.Path)
                        .OutputToFile(tempFile.FullName, true, e => e
                           .WithAudioBitrate(48)
                           .WithAudioCodec("aac")
                           .WithVideoFilters(vf => vf.Scale(-1, 240))
                           .WithVideoCodec("h264_nvenc")
                           .WithVideoBitrate(240)
                           .WithPreset("p7")
                           .WithTune("hq")
                           .UsingThreads(Environment.ProcessorCount)
                        );
                    break;
                case Types.Video360p:
                    break;
                case Types.Video540p:
                    break;
                case Types.Video720p:
                    break;
            }

            Tools.MakePath(tempFile.FullName);

            ffargs?.ProcessSynchronously();
        }
    }
}
