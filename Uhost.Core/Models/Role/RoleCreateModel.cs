using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.Role;
using RoleRightEntity = Uhost.Core.Data.Entities.RoleRight;

namespace Uhost.Core.Models.Role
{
    public class RoleCreateModel : IEntityLoadable<Entity>, IEntityFillable<Entity>
    {
        /// <summary>
        /// Наименование роли
        /// </summary>
        [StringLengthValidation(minLength: 3, maxLength: 64, allowEmpty: false)]
        public string Name { get; set; }

        /// <summary>
        /// Коллекция ИД прав
        /// </summary>
        public IEnumerable<int> RightIds { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Name = Name?.TrimAll() ?? string.Empty;

            if (RightIds != null)
            {
                entity.RoleRights ??= new List<RoleRightEntity>();
                entity.RoleRights.Clear();
                entity.RoleRights.AddRange(RightIds.Distinct().Select(e => new RoleRightEntity { RightId = e }));
            }

            return entity;
        }

        public virtual void LoadFromEntity(Entity entity)
        {
            Name = entity.Name;
            RightIds = entity.RoleRights?.Select(e => e.RightId);
        }
    }
}
