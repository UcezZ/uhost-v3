using System.Collections.Generic;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.VideoReaction;

namespace Uhost.Core.Models.VideoReaction
{
    public class VideoReactionSummaryViewModel
    {
        public IDictionary<Entity.Reactions, int> Reactions { get; set; }

        public IDictionary<Entity.Reactions, UserCommentViewModel> ReactedUsers { get; set; }
    }
}
