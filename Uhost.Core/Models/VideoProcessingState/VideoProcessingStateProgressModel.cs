using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Extensions;
using static Uhost.Core.Data.Entities.File;
using static Uhost.Core.Data.Entities.VideoProcessingState;
using Entity = Uhost.Core.Data.Entities.VideoProcessingState;

namespace Uhost.Core.Models.VideoProcessingState
{
    public sealed class VideoProcessingStateProgressModel : VideoProcessingStateProgressOnlyModel, IEntityCollectionLoadable<Entity>
    {
        public IDictionary<FileTypes, string> States { get; set; }

        public void LoadFromEntityCollection(IQueryable<Entity> entities)
        {
            States = entities.AsEnumerable()
                .OrderBy(e => e.Type.ParseDigits())
                .Select(e => new { Type = e.Type.ParseEnum<FileTypes>(), State = e.State.ParseEnum<VideoProcessingStates>() })
                .Where(e => e.Type != null && e.State != null)
                .ToDictionary(e => e.Type.Value, e => e.State.Value.ToString());
        }
    }
}
