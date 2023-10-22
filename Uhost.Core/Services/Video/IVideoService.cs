using System;
using Uhost.Core.Models.Video;

namespace Uhost.Core.Services.Video
{
    public interface IVideoService : IDisposable, IAsyncDisposable
    {
        Data.Entities.Video Add(VideoCreateModel model);
        void Convert(int id, Data.Entities.File.Types type);
        void Convert(int id, int typeId);
        void Delete(int id);
        object GetAllPaged(VideoQueryModel query);
        VideoViewModel GetOne(int id);
    }
}
