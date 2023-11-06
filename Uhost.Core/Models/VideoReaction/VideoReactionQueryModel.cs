using System.Collections.Generic;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.VideoReaction;

namespace Uhost.Core.Models.VideoReaction
{
    public class VideoReactionQueryModel : PagedQueryModel
    {
        /// <summary>
        /// ИД комментария
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ИД видео
        /// </summary>
        public int VideoId { get; set; }

        /// <summary>
        /// Включая уддалённые
        /// </summary>
        public bool IncludeDeleted { get; set; }

        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }

        internal IEnumerable<int> VideoIds { get; set; }
    }
}
