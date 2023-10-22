using Uhost.Core.Attributes.Validation;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoUpdateModel : BaseModel<Entity>
    {
        /// <summary>
        /// Наименование видео
        /// </summary>
        [StringLengthValidation(minLength: 5, maxLength: 255, allowEmpty: false)]
        public string Name { get; set; }

        /// <summary>
        /// Описание видео
        /// </summary>
        [StringLengthValidation(minLength: 5, maxLength: 5000)]
        public string Description { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            entity.Name = Name ?? string.Empty;
            entity.Description = Description ?? string.Empty;

            return entity;
        }
    }
}
