using System.Collections.Generic;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.Reaction;

namespace Uhost.Core.Models.Reaction
{
    public class ReactionSummaryViewModel
    {
        public IDictionary<Entity.Reactions, int> Reactions { get; set; }

        public IDictionary<Entity.Reactions, IEnumerable<UserCommentViewModel>> ReactedUsers { get; set; }

        public string CurrentUserReaction { get; set; }
    }
}
