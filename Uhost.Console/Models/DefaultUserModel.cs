using System.Collections.Generic;
using System.Linq;
using Uhost.Core;
using Uhost.Core.Extensions;
using Uhost.Core.Models;
using Entity = Uhost.Core.Data.Entities.User;
using UserRoleEntity = Uhost.Core.Data.Entities.UserRole;

namespace Uhost.Console.Models
{
    public class DefaultUserModel : IEntityFillable<Entity>
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string Password { get; set; }
        public IEnumerable<int> RoleIds { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Id = Id;
            entity.Login = Login;

            if (!string.IsNullOrWhiteSpace(Name))
            {
                entity.Name = Name;
            }
            if (!string.IsNullOrEmpty(Description))
            {
                entity.Desctiption = Description;
            }
            if (!string.IsNullOrEmpty(Email))
            {
                entity.Email = Email;
            }
            if (!string.IsNullOrEmpty(Password))
            {
                entity.Password = (Password + CoreSettings.PasswordSalt).ComputeHash(HasherExtensions.EncryptionMethod.SHA256);
            }
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
