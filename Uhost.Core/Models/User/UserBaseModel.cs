using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserBaseModel : IEntityLoadable<Entity>, IEntityFillable<Entity>
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        [StringLengthValidation(maxLength: 64)]
        public string Name { get; set; }

        /// <summary>
        /// Описание
        /// </summary>
        [StringLengthValidation(maxLength: 512)]
        public string Description { get; set; }

        /// <summary>
        /// Имя входа
        /// </summary>
        [RegExpValidation("^[a-zA-Z0-9_]*$"), StringLengthValidation(maxLength: 24)]
        public string Login { get; set; }

        /// <summary>
        /// Тема оформления
        /// </summary>
        [EnumValidation(typeof(Entity.Themes), nameof(Entity.Themes.Dark), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.User_Error_ThemeFail))]
        public string Theme { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Name = Name?.Trim() ?? string.Empty;
            entity.Desctiption = Description?.Trim() ?? string.Empty;
            entity.Login = Login.Trim();
            entity.Theme = Theme.Trim();

            return entity;
        }

        public virtual void LoadFromEntity(Entity entity)
        {
            Name = entity.Name;
            Description = entity.Desctiption;
            Login = entity.Login;
            Theme = entity.Theme;
        }
    }
}
