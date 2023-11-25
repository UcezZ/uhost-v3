using Uhost.Core.Attributes.Validation;
using Entity = Uhost.Core.Data.Entities.Comment;

namespace Uhost.Core.Models.Comment
{
    public class CommentCreateModel : BaseModel<Entity>
    {
        internal int UserId { get; set; }

        internal int VideoId { get; set; }

        /// <summary>
        /// Текст
        /// </summary>
        [StringLengthValidation(minLength: 3, maxLength: 512, allowEmpty: false)]
        public string Text { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            entity.UserId = UserId;
            entity.VideoId = VideoId;
            entity.Text = Text ?? string.Empty;

            return entity;
        }

        public override void LoadFromEntity(Entity entity)
        {
            UserId = entity.UserId;
            VideoId = entity.VideoId;
            Text = entity.Text;
        }
    }
}
