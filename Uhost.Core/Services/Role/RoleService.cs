using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Data;
using Uhost.Core.Extensions;
using Uhost.Core.Models.Role;
using Uhost.Core.Repositories;
using Entity = Uhost.Core.Data.Entities.Role;
using QueryModel = Uhost.Core.Models.Role.RoleQueryModel;

namespace Uhost.Core.Services.Role
{
    public sealed class RoleService : BaseService, IRoleService
    {
        private readonly RoleRepository _repo;
        private readonly RightRepository _rightRepo;

        public RoleService(PostgreSqlDbContext dbContext) : base(dbContext)
        {
            _repo = new RoleRepository(_dbContext);
            _rightRepo = new RightRepository(_dbContext);
        }

        public object GetAllPaged(QueryModel query)
        {
            var pager = _repo.GetAll<RoleViewModel>(query).CreatePager(query);

            return pager.Paginate();
        }

        public RoleViewModel GetOne(int id)
        {
            var model = _repo
                .GetAll<RoleViewModel>(new QueryModel { Id = id })
                .FirstOrDefault();

            return model;
        }

        public Entity Add(RoleCreateModel model)
        {
            return _repo.Add(model);
        }

        public void Update(int id, RoleCreateModel model)
        {
            _repo.Update(id, model);
        }

        public void Delete(int id)
        {
            _repo.SoftDelete(id);
        }

        public bool Exists(string name, int excludedId = 0)
        {
            var query = new QueryModel
            {
                Name = name,
                ExcludedId = excludedId
            };

            return _repo.PrepareQuery(query).Any();
        }

        public bool CheckRightIds(IEnumerable<int> ids, out int invalid)
        {
            return _rightRepo.CheckIds(ids, out invalid);
        }
    }
}
