using System.Collections.Generic;
using static Uhost.Core.Data.Entities.File;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoViewModel : VideoShortViewModel
    {
        public IDictionary<Types, string> Urls { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Description = entity.Description;
        }
    }
}
