using System.Collections.Generic;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoQueryModel : PagedQueryModel
    {
        /// <summary>
        /// ИД видео
        /// </summary>
        public int Id { get; set; }

        internal IEnumerable<int> Ids { get; set; }

        /// <summary>
        /// Токен видео
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Имя - триграмсы или вхождение
        /// </summary>
        [StringLengthValidation(3)]
        public string Name { get; set; }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Точный поиск по логину пользователя
        /// </summary>
        public string UserLogin { get; set; }

        /// <summary>
        /// Включая удалённые
        /// </summary>
        public bool IncludeDeleted { get; set; }

        /// <summary>
        /// Разрешены комментарии
        /// </summary>
        public bool? AllowComments { get; set; }

        /// <summary>
        /// Разрешены реакции
        /// </summary>
        public bool? AllowReactions { get; set; }

        /// <summary>
        /// Показывать скрытые из поиска
        /// </summary>
        public bool ShowHidden { get; set; }

        /// <summary>
        /// Показывать приватные
        /// </summary>
        public bool ShowPrivate { get; set; }

        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }

        internal Entity.SortBy? SortByParsed => SortBy.ParseEnum<Entity.SortBy>();

        internal bool IncludeProcessingStates { get; set; }

        /// <summary>
        /// Показывать скрытые видео этого пользователя, если не показываются
        /// </summary>
        internal int ShowHiddenForUserId { get; set; }

        /// <summary>
        /// Показывать приватные видео этого пользователя, если не показываются
        /// </summary>
        internal int ShowPrivateForUserId { get; set; }

        internal bool ForceShowForUser { get; set; }
    }
}
