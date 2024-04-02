using Uhost.Core.Models;
using Uhost.Core.Models.Playlist;
using Entity = Uhost.Core.Data.Entities.Playlist;
using QueryModel = Uhost.Core.Models.Playlist.PlaylistQueryModel;

namespace Uhost.Core.Services.Playlist
{
    public interface IPlaylistService
    {
        Entity Create(PlaylistCreateModel model);
        void Delete(int id);
        PagerResultModel<PlaylistViewModel> GetAllPaged(QueryModel query);
        PlaylistViewModel GetOne(int id);
        void Update(int id, PlaylistUpdateModel model);
    }
}
