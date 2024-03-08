using System.Collections.Generic;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.File
{
    public class FileQueryModel : PagedQueryModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int DynId { get; set; }

        public IEnumerable<int> DynIds { get; set; }

        [EnumValidation(typeof(Entity.FileTypes), allowEmpty: true)]
        public string Type { get; set; }

        [EnumValidation(typeof(Entity.FileTypes), allowEmpty: true)]
        public IEnumerable<string> Types { get; set; }

        [EntityNameValidation(true)]
        public string DynName { get; set; }

        public string Token { get; set; }

        public bool IncludeDeleted { get; set; }

        [EnumValidation(typeof(Entity.SortBy), nameof(Entity.SortBy.Id), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_SortBy))]
        public override string SortBy { get; set; }
    }
}
