using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uhost.Core.Models.Video;
using static Uhost.Core.Data.Entities.File;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Services.Video
{
    public interface IVideoService : IDisposable, IAsyncDisposable
    {
        Entity Add(VideoUploadFileModel model);
        Entity Add(VideoUploadUrlModel model);
        Task Convert(int id, Types type);
        Task ConvertUrl(int id, Types type, string url, TimeSpan maxDuration);
        void Delete(int id);
        void Delete(string token);
        object GetAllPaged(VideoQueryModel query);
        Task<IDictionary<Types, double>> GetConversionProgress(string token);
        VideoViewModel GetOne(int id);
        VideoViewModel GetOne(string token);
        void Update(string token, VideoUpdateModel model);
    }
}
