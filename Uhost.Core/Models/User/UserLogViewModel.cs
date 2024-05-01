using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public sealed class UserLogViewModel : IEntityLoadable<Entity>
    {
        internal int Id { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public string LastVisitAt { get; set; }

        public void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Login = entity.Login;
            Email = entity.Email;
            LastVisitAt = entity.LastVisitAt?.ToHumanFmt();
        }
    }
}
