using System.Collections.Generic;
using Entity = Uhost.Core.Data.Entities.Right;

namespace Uhost.Core.Models.Right
{
    public class RightQueryModel : BaseQueryModel
    {
        public IEnumerable<int> RoleIds { get; set; }

        public override string SortBy
        {
            get => nameof(Entity.Id);
            set { }
        }
    }
}
