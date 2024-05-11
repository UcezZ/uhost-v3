using System;
using Uhost.Core.Models.Reaction;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Reaction;

namespace Uhost.Core.Services.Reaction
{
    public interface IReactionService : IDisposable
    {
        bool AreReactionsAllowed(string videoToken);
        bool CheckUserRestrictions(string videoToken, out Rights missing);
        ReactionSummaryViewModel GetOne(string videoToken);
        bool Remove(string videoToken);
        ReactionSummaryViewModel RemoveAndGetStats(string videoToken);
        Entity Set(string videoToken, Entity.Reactions reaction);
        ReactionSummaryViewModel SetAndGetStats(string videoToken, Entity.Reactions reaction);
    }
}
