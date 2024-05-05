using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.File;
using Uhost.Core.Models.Right;
using Uhost.Core.Models.Role;
using Uhost.Core.Models.User;
using Uhost.Core.Repositories;
using Uhost.Core.Services.File;
using Uhost.Core.Services.Log;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.User;
using QueryModel = Uhost.Core.Models.User.UserQueryModel;

namespace Uhost.Core.Services.User
{
    public sealed class UserService : BaseService, IUserService
    {
        private readonly RoleRepository _roleRepo;
        private readonly UserRepository _repo;
        private readonly RightRepository _rightRepo;
        private readonly IFileService _files;
        private readonly ILogService _log;

        public UserService(
            IDbContextFactory<PostgreSqlDbContext> factory,
            IServiceProvider provider,
            IFileService files,
            ILogService log) : base(factory, provider)
        {
            _repo = new UserRepository(_dbContext);
            _roleRepo = new RoleRepository(_dbContext);
            _rightRepo = new RightRepository(_dbContext);
            _files = files;
            _log = log;
        }

        public UserAccessModel GetAccessData(int id)
        {
            var model = _repo
                .GetAll<UserAccessModel>(new QueryModel { Id = id })
                .FirstOrDefault();

            if (model != null)
            {
                model.Roles = _roleRepo
                    .GetAll<RoleViewModel>(new RoleQueryModel { UserId = id })
                    .ToList();
            }

            return model;
        }

        public object GetAllPaged(QueryModel query)
        {
            var pager = _repo
                .GetAll<UserShortViewModel>(query)
                .CreatePager(query);

            if (pager.Any())
            {
                var avatars = _files
                    .GetByDynEntity<FileShortViewModel>(pager.Select(e => e.Id), typeof(Entity), FileTypes.UserAvatar)
                    .ToList();

                foreach (var model in pager)
                {
                    model.AvatarUrl = avatars.FirstOrDefault(e => e.DynId == model.Id)?.Url;
                }

                avatars.Clear();
            }

            return pager.Paginate();
        }

        public UserViewModel GetOne(int id) => GetOne(new QueryModel { Id = id });

        public UserViewModel GetOne(string login) => GetOne(new QueryModel { Login = login });

        private UserViewModel GetOne(QueryModel query)
        {
            query.IncludePlaylists = true;
            query.IncludeVideos = true;

            var model = _repo
                .GetAll<UserViewModel>(query)
                .FirstOrDefault();

            if (model != null)
            {
                model.Roles = _roleRepo
                    .GetAll<RoleShortViewModel>(new RoleQueryModel { UserId = model.Id })
                    .ToList();
                model.Rights = _rightRepo
                    .GetAll<RightViewModel>(new RightQueryModel { RoleIds = model.Roles.Select(e => e.Id) })
                    .ToList();
                model.AvatarUrl = _files
                    .GetByDynEntity<FileShortViewModel>(model.Id, typeof(Entity), FileTypes.UserAvatar)
                    .FirstOrDefault()?.Url;
            }

            return model;
        }

        public Entity Add(UserCreateModel model)
        {
            return _repo.Add(model);
        }

        public Entity Add(UserRegisterModel model)
        {
            return _repo.Add(model);
        }

        public void Update(int id, UserPasswordUpdateModel model)
        {
            _repo.Update(id, model);
        }

        public void Update(int id, UserCreateModel model)
        {
            _repo.Update(id, model);
        }

        public void Update(int id, UserSelfUpdateModel model)
        {
            _repo.Update(id, model);
        }

        public void Delete(int id)
        {
            _repo.SoftDelete(id);
        }

        public bool Exists(string login, string email, int excludedId = 0)
        {
            var query = new QueryModel
            {
                ExcludedId = excludedId
            };

            return _repo
                .PrepareQuery(query)
                .Any(e => e.Login == login || e.Email == email);
        }

        public Entity Auth(UserLoginQueryModel query)
        {
            var entities = _repo
                .PrepareQuery(new QueryModel { LoginOrEmail = query.Login })
                .ToList();

            var password = (query.Password + CoreSettings.PasswordSalt).ComputeHash(HasherExtensions.EncryptionMethod.SHA256);

            return entities.FirstOrDefault(e => e.Password == password);
        }

        /// <summary>
        /// Загрузка аватара пользователя
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string UploadAvatar(UserUpdateAvatarModel model, int userId = default)
        {
            if (userId <= 0 && !TryGetUserId(out userId))
            {
                return null;
            }

            using (var temp = new MemoryStream())
            {
                using (var webStream = model.File.OpenReadStream())
                {
                    webStream.CopyTo(temp);
                }

                temp.Position = 0;

                if (temp.TryShrinkImage(400, 50))
                {
                    var entity = _files.Add(
                        temp,
                        "avatar.jpg",
                        type: FileTypes.UserAvatar,
                        dynName: typeof(Entity).Name,
                        dynId: userId);
                    _log.Add(Events.UserAvatarUploaded);

                    return entity?.GetUrl();
                }
            }

            return null;
        }

        /// <summary>
        /// Удаление аватара пользователя
        /// </summary>
        /// <param name="userId"></param>
        public void DeleteAvatar(int userId = default)
        {
            if (userId <= 0 && !TryGetUserId(out userId))
            {
                return;
            }

            _files.DeleteByDynParams(userId, typeof(Entity), FileTypes.UserAvatar);
        }

        public int UpdateLastVisitAt(int id)
        {
            return _repo.UpdateLastVisitAt(id);
        }
    }
}
