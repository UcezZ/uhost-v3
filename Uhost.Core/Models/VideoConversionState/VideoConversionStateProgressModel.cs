using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.VideoConversionState;
using Entity = Uhost.Core.Data.Entities.VideoConversionState;

namespace Uhost.Core.Models.VideoConversionState
{
    public sealed class VideoConversionStateProgressModel : VideoConversionStateProgressOnlyModel, IEntityCollectionLoadable<Entity>
    {
        public IDictionary<FileTypes, string> States { get; set; }

        public void LoadFromEntityCollection(IQueryable<Entity> entities)
        {
            States = entities.AsEnumerable()
                .Select(e => new { Type = e.Type.ParseEnum<FileTypes>(), State = e.State.ParseEnum<VideoConversionStates>() })
                .Where(e => e.Type != null && e.State != null)
                .ToDictionary(e => e.Type.Value, e => e.State.Value.ToString());
        }
    }
}
