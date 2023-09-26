using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Uhost.Core.Data.Entities.User;
using UserRoleEntity = Uhost.Core.Data.Entities.UserRole;

namespace Uhost.Core.Models.User
{
    public class UserCreateModel : UserBaseModel
    {
        /// <summary>
        /// Коллекция ИД ролей
        /// </summary>
        public IEnumerable<int> RoleIds { get; set; } = Array.Empty<int>();

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.UserRoles = RoleIds
                .Distinct()
                .Select(e => new UserRoleEntity { RoleId = e })
                .ToList();

            return entity;
        }
    }
}
