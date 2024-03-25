using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Models.Role;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserAccessModel : IEntityLoadable<Entity>
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public IEnumerable<RoleViewModel> Roles { get; set; }

        [JsonIgnore]
        public IEnumerable<Rights> Rights => Roles.SelectMany(e => e.RightsCasted).Distinct();

        public virtual void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Login = entity.Login;
        }
    }
}
