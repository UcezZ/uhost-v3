using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.Comment;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.Comment
{
    public class CommentViewModel : CommentCreateModel
    {
        public UserCommentViewModel User { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);
            
            User = entity.User?.ToModel<UserEntity, UserCommentViewModel>();
        }
    }
}
