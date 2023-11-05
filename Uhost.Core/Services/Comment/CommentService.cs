using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Comment;
using Uhost.Core.Models.File;
using Uhost.Core.Models.User;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using static Uhost.Core.Data.Entities.File;
using Entity = Uhost.Core.Data.Entities.Comment;
using QueryModel = Uhost.Core.Models.Comment.CommentQueryModel;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Services.Comment
{
    public class CommentService : BaseService, ICommentService
    {
        private readonly CommentRepository _repo;
        private readonly UserRepository _userRepo;
        private readonly IFileService _fileService;
        private readonly IHttpContextAccessor _contextAccessor;

        public CommentService(PostgreSqlDbContext dbContext, IServiceProvider provider, IFileService fileService) : base(dbContext)
        {
            _repo = new CommentRepository(dbContext);
            _fileService = fileService;
            _contextAccessor = provider.GetService<IHttpContextAccessor>();
        }

        public object GetAllPaged(QueryModel query)
        {
            var pager = _repo
                .GetAll<CommentViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var users = _userRepo
                    .GetAll<UserCommentViewModel>(new UserQueryModel { Ids = pager.Select(e => e.UserId) })
                    .ToList();

                var files = _fileService
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.UserId), typeof(UserEntity), Types.UserAvatar)
                    .ToList();

                foreach (var model in users)
                {
                    model.AvatarUrl = files.FirstOrDefault(e => e.DynId == model.Id)?.Url;
                }

                foreach (var model in pager)
                {
                    model.User = users.FirstOrDefault(e => e.Id == model.UserId);
                }
            }

            return pager.Paginate();
        }

        public Entity Add(CommentCreateModel model)
        {
            if (_contextAccessor?.HttpContext?.User != null && _contextAccessor.HttpContext.User.TryGetUserId(out var userId))
            {
                model.UserId = userId;
            }

            return _repo.Add(model);
        }

        public bool Delete(string videoToken, int id)
        {
            var affected = _repo.Perform(e => e.DeletedAt = DateTime.Now, e => e.Id == id && e.DeletedAt == null && e.Video.Token == videoToken);

            return affected > 0;
        }
    }
}
