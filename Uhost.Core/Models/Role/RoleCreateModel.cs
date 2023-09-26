using System;
using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.Role;
using RoleRightEntity = Uhost.Core.Data.Entities.RoleRight;

namespace Uhost.Core.Models.Role
{
    public class RoleCreateModel : BaseModel<Entity>
    {
        /// <summary>
        /// Наименование роли
        /// </summary>
        [StringLengthValidation(minLength: 3, maxLength: 64, allowEmpty: false)]
        public string Name { get; set; }

        /// <summary>
        /// Коллекция ИД прав
        /// </summary>
        public IEnumerable<int> RightIds { get; set; } = Array.Empty<int>();

        public override Entity FillEntity(Entity entity)
        {
            entity.Name = Name?.TrimAll() ?? string.Empty;
            entity.RoleRights = RightIds
                .Distinct()
                .Select(e => new RoleRightEntity { RightId = e })
                .ToList();

            return entity;
        }

        public override void LoadFromEntity(Entity entity)
        {
            Name = entity.Name;
            RightIds = entity.RoleRights?.Select(e => e.RightId);
        }
    }
}
