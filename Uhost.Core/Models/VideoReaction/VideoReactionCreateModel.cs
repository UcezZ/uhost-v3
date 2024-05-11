using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Reaction;
using VideoEntity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Reaction
{
    public class ReactionCreateModel : IEntityFillable<Entity>
    {
        internal int UserId { get; set; }

        /// <summary>
        /// ИД видео
        /// </summary>
        [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Video_Error_NotFoundById))]
        public int VideoId { get; set; }

        /// <summary>
        /// Реакция
        /// </summary>
        [EnumValidation(typeof(Entity.Reactions))]
        public string Value { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.UserId = UserId;
            entity.VideoId = VideoId;
            entity.Value = Value;

            return entity;
        }
    }
}
