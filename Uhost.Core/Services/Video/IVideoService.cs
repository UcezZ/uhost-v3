﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Uhost.Core.Models;
using Uhost.Core.Models.Video;
using Uhost.Core.Models.VideoProcessingState;
using static Uhost.Core.Data.Entities.File;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Services.Video
{
    public interface IVideoService : IDisposable
    {
        Entity Add(VideoUploadFileModel model);
        Entity Add(VideoUploadUrlModel model);
        void Delete(int id);
        void Delete(string token);
        PagerResultModel<VideoShortViewModel> GetAllPaged(VideoQueryModel query);
        Task<VideoProcessingStateProgressModel> GetConversionProgressAsync(string token);
        VideoViewModel GetOne(int id);
        void Update(string token, VideoUpdateModel model);
        void OverrideByUserRestrictions(VideoQueryModel query);
        Task FetchStream(int id, string url);
        IEnumerable<VideoShortViewModel> GetRandom(int count);
        Task<VideoViewModel> GetOne(string token);
        Task Convert(int id);
        bool TryGetDownload(string token, FileTypes type, out string name, out Stream stream, out DateTime lastModified);
        PagerResultModel<VideoShortProcessingModel> GetAllProcessingsPaged(VideoQueryModel query);
    }
}
