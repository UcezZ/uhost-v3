using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uhost.Core.Models.Video;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Services.Video
{
    public interface IVideoService : IDisposable
    {
        Entity Add(VideoUploadFileModel model);
        Entity Add(VideoUploadUrlModel model, out bool infinite);
        void Delete(int id);
        void Delete(string token);
        object GetAllPaged(VideoQueryModel query);
        Task<VideoConversionProgressModel> GetConversionProgressAsync(string token);
        VideoViewModel GetOne(int id);
        VideoViewModel GetOne(string token);
        void Update(string token, VideoUpdateModel model);
        void OverrideByUserRestrictions(VideoQueryModel query);
        Task FetchUrl(int id, string url);
        Task Convert(int id, Data.Entities.File.Types type);
        IEnumerable<VideoShortViewModel> GetRandom(int count);
    }
}
