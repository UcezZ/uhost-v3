using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.File;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.File
{
    public class FileViewModel : FileShortViewModel
    {
        public UserShortViewModel User { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            User = entity.User?.ToModel<UserEntity, UserShortViewModel>();
        }
    }
}
