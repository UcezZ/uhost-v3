﻿using Entity = Uhost.Core.Data.Entities.User;

namespace Uhost.Core.Models.User
{
    public class UserCommentViewModel : IEntityLoadable<Entity>
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }

        public virtual void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Login = entity.Login;
            Name = entity.Name;
        }
    }
}
