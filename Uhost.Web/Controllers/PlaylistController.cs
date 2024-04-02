using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Playlist;
using Uhost.Core.Services.Playlist;
using Uhost.Web.Common;
using Uhost.Web.Extensions;
using Uhost.Web.Properties;
using Entity = Uhost.Core.Data.Entities.Playlist;
using QueryModel = Uhost.Core.Models.Playlist.PlaylistQueryModel;

namespace Uhost.Web.Controllers
{
    /// <summary>
    /// Плейлисты
    /// </summary>
    [Route("api/v2/playlists"), Authorize]
    public class PlaylistController : Controller
    {
        private readonly IPlaylistService _service;

        public PlaylistController(IPlaylistService service)
        {
            _service = service;
        }

        /// <summary>
        /// Получение всех поейлистов по запросу
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public IActionResult GetAll(QueryModel query)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            return ResponseHelper.Success(_service.GetAllPaged(query));
        }

        /// <summary>
        /// Получение одного плейлиста по ИД
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), AllowAnonymous]
        public IActionResult GetOne(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Playlist_Error_NotFoundById))]
            string id)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var model = _service.GetOne(idParsed);

            if (model == null)
            {
                return ResponseHelper.Error(ApiStrings.Playlist_Error_NotFoundById);
            }
            else
            {
                return ResponseHelper.Success(model);
            }
        }

        /// <summary>
        /// Создание плейлиста
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create([FromBody] PlaylistCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            var entity = _service.Create(model);

            return ResponseHelper.Success(_service.GetOne(entity.Id));
        }

        /// <summary>
        /// Обновление плейлиста с содержимым
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Update(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Playlist_Error_NotFoundById))]
            string id,
            [FromBody]
            PlaylistUpdateModel model)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Update(idParsed, model);

            return ResponseHelper.Success(_service.GetOne(idParsed));
        }

        /// <summary>
        /// Удаление плейлиста
        /// </summary>
        /// <param name="id">ИД плейлиста</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(
            [DatabaseExistionValidation(typeof(Entity), nameof(Entity.Id), ErrorMessageResourceType = typeof(ApiStrings), ErrorMessageResourceName = nameof(ApiStrings.Playlist_Error_NotFoundById))]
            string id)
        {
            if (!id.TryParsePositiveInt(out var idParsed))
            {
                return ResponseHelper.ErrorMessage(nameof(id), ApiStrings.Common_Error_Invalid);
            }
            if (!ModelState.IsValid)
            {
                return ResponseHelper.Error(ModelState.GetErrors());
            }

            _service.Delete(idParsed);

            return ResponseHelper.Success();
        }
    }
}
