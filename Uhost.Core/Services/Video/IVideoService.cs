using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Uhost.Core.Models.Video;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Services.Video
{
    public interface IVideoService : IDisposable, IAsyncDisposable
    {
        Entity Add(VideoCreateModel model);
        Task Convert(int id, Data.Entities.File.Types type);
        Task Convert(int id, int typeId);
        void Delete(int id);
        object GetAllPaged(VideoQueryModel query);
        Task<IDictionary<Data.Entities.File.Types, double>> GetConversionProgress(int videoId);
        VideoViewModel GetOne(int id);
    }
}
