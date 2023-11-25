using System;
using Uhost.Core.Models.Comment;
using Entity = Uhost.Core.Data.Entities.Comment;
using QueryModel = Uhost.Core.Models.Comment.CommentQueryModel;

namespace Uhost.Core.Services.Comment
{
    public interface ICommentService : IDisposable, IAsyncDisposable
    {
        Entity Add(string videoToken, string text);
        bool AreCommentsAllowed(string videoToken);
        bool CheckUserRestrictions(string videoToken, out Data.Entities.Right.Rights missing);
        bool Delete(string videoToken, int id);
        object GetAllPaged(QueryModel query);
        CommentViewModel GetOne(int id);
    }
}
