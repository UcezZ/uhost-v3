﻿using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
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
        [RegExpValidation("^[a-zA-Z0-9\\-_\\.]*$"), StringLengthValidation(maxLength: 32, allowEmpty: false)]
        public string Login { get; set; }

        /// <summary>
        /// Тема оформления
        /// </summary>
        [EnumValidation(typeof(Entity.Themes), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.User_Error_ThemeFail))]
        public string Theme { get; set; }

        /// <summary>
        /// Язык оформления
        /// </summary>
        [EnumValidation(typeof(Entity.Locales), ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.User_Error_LocaleFail))]
        public string Locale { get; set; }

        internal Entity.Locales? LocaleParsed => Locale?.ParseEnum<Entity.Locales>();

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Login = Login?.Trim() ?? string.Empty;
            entity.Name = Name?.Trim() ?? string.Empty;
            entity.Desctiption = Description?.FilterWebMultilineString() ?? string.Empty;
            entity.Theme = Theme?.Trim() ?? string.Empty;
            entity.Locale = Locale?.Trim() ?? string.Empty;

            return entity;
        }

        public virtual void LoadFromEntity(Entity entity)
        {
            Name = entity.Name;
            Description = entity.Desctiption;
            Login = entity.Login;
            Theme = entity.Theme;
            Locale = entity.Locale;
        }
    }
}
