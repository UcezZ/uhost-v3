using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.VideoProcessingState;

namespace Uhost.Core.Models.VideoProcessingState
{
    public class VideoProcessingStateQueryModel : PagedQueryModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int VideoId { get; set; }

        public Entity.VideoProcessingStates? State { get; set; }

        public string Token { get; set; }

        public bool IncludeDeleted { get; set; }

        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.CreatedAt), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }

        internal Entity.SortBy? SortByParsed => SortBy?.ParseEnum<Entity.SortBy>();
    }
}
