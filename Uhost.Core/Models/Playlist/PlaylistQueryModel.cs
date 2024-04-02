using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Playlist;

namespace Uhost.Core.Models.Playlist
{
    public class PlaylistQueryModel : PagedQueryModel
    {
        /// <summary>
        /// ИД плейлиста
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Точное совпадение с логином ползователя
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// Включая удалённые
        /// </summary>
        public bool IncludeDeleted { get; set; }

        /// <inheritdoc cref="BaseQueryModel.SortBy"/>
        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }

        internal Entity.SortBy? SortByParsed => SortBy?.ParseEnum<Entity.SortBy>();
    }
}
