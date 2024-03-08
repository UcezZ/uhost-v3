﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uhost.Core.Models.Video;
using Uhost.Core.Models.VideoConversionState;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Services.Video
{
    public interface IVideoService : IDisposable
    {
        Entity Add(VideoUploadFileModel model);
        Entity Add(VideoUploadUrlModel model);
        void Delete(int id);
        void Delete(string token);
        object GetAllPaged(VideoQueryModel query);
        Task<VideoConversionStateProgressModel> GetConversionProgressAsync(string token);
        VideoViewModel GetOne(int id);
        void Update(string token, VideoUpdateModel model);
        void OverrideByUserRestrictions(VideoQueryModel query);
        Task FetchStream(int id, string url);
        IEnumerable<VideoShortViewModel> GetRandom(int count);
        Task<VideoViewModel> GetOne(string token);
        Task Convert(int id);
    }
}
