using System.Collections.Generic;
using Uhost.Core.Attributes.Validation;
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
        /// Включая удалённые
        /// </summary>
        public bool IncludeDeleted { get; set; }

        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }
    }
}
