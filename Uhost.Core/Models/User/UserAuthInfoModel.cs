using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserAuthInfoModel : UserAccessModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public string Theme { get; set; }

        public string LastVisitAt { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Name = entity.Name;
            Description = entity.Desctiption;
            Email = entity.Email;
            Theme = entity.Theme;
            LastVisitAt = entity.LastVisitAt?.ToApiFmt();
        }
    }
}
