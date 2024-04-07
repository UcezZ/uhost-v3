using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.Comment;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.Comment
{
    public class CommentViewModel : CommentCreateModel
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public UserCommentViewModel User { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Id = entity.Id;
            CreatedAt = entity.CreatedAt.ToHumanFmt();
            User = entity.User?.ToModel<UserEntity, UserCommentViewModel>();
        }
    }
}
