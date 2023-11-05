using Uhost.Core.Models.User;

namespace Uhost.Core.Models.Comment
{
    public class CommentViewModel : CommentCreateModel
    {
        public UserCommentViewModel User { get; set; }
    }
}
