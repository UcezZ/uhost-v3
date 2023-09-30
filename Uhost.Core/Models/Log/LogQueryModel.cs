using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Models.Log
{
    public class LogQueryModel : PagedQueryModel
    {
        public int UserId { get; set; }

        [EnumValidation(typeof(Events), allowEmpty: true)]
        public IEnumerable<string> Events { get; set; }

        internal IEnumerable<Events> EventsParsed => Events?.Select(e => e.ParseEnum<Events>()).OfType<Events>();

        [EnumValidation(typeof(SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }
    }
}
