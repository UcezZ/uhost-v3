using System.Collections.Generic;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoQueryModel : PagedQueryModel
    {
        public int Id { get; set; }

        internal IEnumerable<int> Ids { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public bool IncludeDeleted { get; internal set; }

        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }
    }
}
