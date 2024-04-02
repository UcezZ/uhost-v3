using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserShortViewModel : UserBaseModel
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string LastVisitAt { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Id = entity.Id;
            CreatedAt = entity.CreatedAt.ToHumanFmt();
            LastVisitAt = entity.LastVisitAt?.ToHumanFmt();
            Email = entity.Email;
        }
    }
}
