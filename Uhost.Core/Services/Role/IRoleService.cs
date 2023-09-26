using System;
using System.Collections.Generic;
using Uhost.Core.Models.Role;
using Entity = Uhost.Core.Data.Entities.Role;

namespace Uhost.Core.Services.Role
{
    public interface IRoleService : IDisposable, IAsyncDisposable
    {
        Entity Add(RoleCreateModel model);
        bool CheckRightIds(IEnumerable<int> ids, out int invalid);
        void Delete(int id);
        bool Exists(string name, int excludedId = 0);
        object GetAllPaged(RoleQueryModel query);
        RoleViewModel GetOne(int id);
        void Update(int id, RoleCreateModel model);
    }
}
