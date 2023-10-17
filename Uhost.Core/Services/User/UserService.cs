using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Right;
using Uhost.Core.Models.Role;
using Uhost.Core.Models.User;
using Uhost.Core.Repositories;
using Entity = Uhost.Core.Data.Entities.User;
using QueryModel = Uhost.Core.Models.User.UserQueryModel;

namespace Uhost.Core.Services.User
{
    public sealed class UserService : BaseService, IUserService
    {
        private readonly RoleRepository _roleRepo;
        private readonly UserRepository _repo;
        private readonly RightRepository _rightRepo;

        public UserService(PostgreSqlDbContext dbContext) : base(dbContext)
        {
            _repo = new UserRepository(_dbContext);
            _roleRepo = new RoleRepository(_dbContext);
            _rightRepo = new RightRepository(_dbContext);
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

            return pager.Paginate();
        }

        public UserViewModel GetOne(int id)
        {
            var model = _repo
                .GetAll<UserViewModel>(new QueryModel { Id = id })
                .FirstOrDefault();

            if (model != null)
            {
                model.Roles = _roleRepo
                    .GetAll<RoleShortViewModel>(new RoleQueryModel { UserId = id })
                    .ToList();
                model.Rights = _rightRepo
                    .GetAll<RightViewModel>(new RightQueryModel { RoleIds = model.Roles.Select(e => e.Id) })
                    .ToList();
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

        public void Update(int id, UserBaseModel model)
        {
            _repo.Update(id, model);
        }

        public void Delete(int id)
        {
            _repo.SoftDelete(id);
        }

        public bool Exists(string login, int excludedId = 0)
        {
            var query = new QueryModel
            {
                Login = login,
                ExcludedId = excludedId
            };

            return _repo.PrepareQuery(query).Any();
        }

        public bool CheckRoleIds(IEnumerable<int> ids, out int invalid)
        {
            return _roleRepo.CheckIds(ids, out invalid);
        }

        public Entity Auth(UserLoginQueryModel query)
        {
            var entities = _repo
                .PrepareQuery(new QueryModel { LoginOrEmail = query.Login })
                .ToList();

            var password = (query.Password + CoreSettings.PasswordSalt).ComputeHash(HasherExtensions.EncryptionMethod.SHA256);

            return entities.FirstOrDefault(e => e.Password == password);
        }

        public int UpdateLastVisitAt(int id)
        {
            return _repo.UpdateLastVisitAt(id);
        }
    }
}
