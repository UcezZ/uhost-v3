using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoShortQueryModel : PagedQueryModel
    {
        /// <summary>
        /// Имя - триграмсы или вхождение
        /// </summary>
        [StringLengthValidation(3)]
        public string Name { get; set; }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        internal int UserId { get; set; }

        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }
    }
}
