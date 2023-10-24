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
        Entity Add(VideoCreateModel model);
        Task Convert(int id, Types type);
        void Delete(int id);
        object GetAllPaged(VideoQueryModel query);
        Task<IDictionary<Types, double>> GetConversionProgress(int videoId);
        VideoViewModel GetOne(int id);
    }
}
