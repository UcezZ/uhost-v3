using System.Collections.Generic;
using System.Linq;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Uhost.Core.Properties;
using static Uhost.Core.Data.Entities.Right;
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
        [EnumValidation(typeof(Rights), allowEmpty: true, ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Right_Error_NotFound))]
        public IEnumerable<string> Rights { get; set; }

        internal IEnumerable<Rights> RightsParsed => Rights.Select(e => e.ParseEnum<Rights>()).OfType<Rights>();

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Name = Name?.TrimAll() ?? string.Empty;

            if (Rights != null)
            {
                entity.RoleRights ??= new List<RoleRightEntity>();
                entity.RoleRights.Clear();
                entity.RoleRights.AddRange(RightsParsed.Distinct().Select(e => new RoleRightEntity { RightId = (int)e }));
            }

            return entity;
        }

        public virtual void LoadFromEntity(Entity entity)
        {
            Name = entity.Name;
            Rights = entity.RoleRights?.Select(e => ((Rights)e.RightId).ToString()).ToList();
        }
    }
}
