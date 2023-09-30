using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.User;
using UserRoleEntity = Uhost.Core.Data.Entities.UserRole;

namespace Uhost.Core.Models.User
{
    public class UserCreateModel : UserBaseModel
    {
        /// <summary>
        /// Коллекция ИД ролей
        /// </summary>
        public IEnumerable<int> RoleIds { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            if (RoleIds != null)
            {
                entity.UserRoles ??= new List<UserRoleEntity>();
                entity.UserRoles.Clear();
                entity.UserRoles.AddRange(RoleIds.Distinct().Select(e => new UserRoleEntity { RoleId = e }).ToList());
            }

            return entity;
        }
    }
}
