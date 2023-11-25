using System;
using Uhost.Core.Models.VideoReaction;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.VideoReaction;

namespace Uhost.Core.Services.VideoReaction
{
    public interface IVideoReactionService : IDisposable, IAsyncDisposable
    {
        bool AreReactionsAllowed(string videoToken);
        bool CheckUserRestrictions(string videoToken, out Rights missing);
        VideoReactionSummaryViewModel GetOne(string videoToken);
        bool Remove(string videoToken);
        Entity Set(string videoToken, Entity.Reactions reaction);
    }
}
