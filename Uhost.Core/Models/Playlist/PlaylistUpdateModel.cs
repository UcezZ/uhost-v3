using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Playlist;
using EntryEntity = Uhost.Core.Data.Entities.PlaylistEntry;
using VideoEntity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Playlist
{
    public class PlaylistUpdateModel : PlaylistBaseModel
    {
        /// <summary>
        /// Коллекция ИД видео
        /// </summary>
        [DatabaseExistionValidation(typeof(VideoEntity), nameof(VideoEntity.Id), nullable: true, ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Video_Error_NotFoundById))]
        public ICollection<int> VideoIds { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.PlaylistEntries ??= new List<EntryEntity>();
            entity.PlaylistEntries.Clear();
            entity.PlaylistEntries.AddRange(VideoIds
                .Select((e, i) => new EntryEntity
                {
                    VideoId = e,
                    Order = i
                }));

            return entity;
        }
    }
}
