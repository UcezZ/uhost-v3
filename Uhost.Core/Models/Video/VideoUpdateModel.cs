﻿using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoUpdateModel : IEntityFillable<Entity>
    {
        /// <summary>
        /// Наименование видео
        /// </summary>
        [StringLengthValidation(minLength: 5, maxLength: 255, allowEmpty: false)]
        public string Name { get; set; }

        /// <summary>
        /// Описание видео
        /// </summary>
        [StringLengthValidation(minLength: 5, maxLength: 5000)]
        public string Description { get; set; }

        public bool IsPrivate { get; set; }

        public bool IsHidden { get; set; }

        public bool AllowComments { get; set; }

        public bool AllowReactions { get; set; }

        public bool LoopPlayback { get; set; }

        public virtual Entity FillEntity(Entity entity)
        {
            entity.Name = Name ?? string.Empty;
            entity.Description = Description?.FilterWebMultilineString() ?? string.Empty;
            entity.IsPrivate = IsPrivate;
            entity.IsHidden = IsHidden;
            entity.AllowComments = AllowComments;
            entity.AllowReactions = AllowReactions;
            entity.LoopPlayback = LoopPlayback;

            return entity;
        }
    }
}
