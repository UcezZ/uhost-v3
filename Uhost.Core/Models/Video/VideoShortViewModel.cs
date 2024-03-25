using System;
using System.Collections.Generic;
using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Entity = Uhost.Core.Data.Entities.Video;
using UserEntity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.Video
{
    public class VideoShortViewModel : IEntityLoadable<Entity>
    {
        internal int Id { get; set; }

        public string Name { get; set; }

        public string CreatedAt { get; set; }

        public string Description { get; set; }

        public string Duration => DurationObj.ToHumanFmt();

        public string Token { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool IsHidden { get; set; }

        public bool IsPrivate { get; set; }

        public IEnumerable<string> Resolutions { get; set; }

        public int UserId { get; set; }

        public UserShortViewModel User { get; set; }

        internal TimeSpan DurationObj { get; set; }

        public virtual void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CreatedAt = entity.CreatedAt.ToApiFmt();
            Description = entity.Description.Length > 32 ? entity.Description[..32] : entity.Description;
            DurationObj = entity.Duration;
            Token = entity.Token;
            UserId = entity.UserId;
            IsPrivate = entity.IsPrivate;
            IsHidden = entity.IsHidden;
            User = entity.User?.ToModel<UserEntity, UserShortViewModel>();
        }
    }
}
