using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using static Uhost.Core.Data.Entities.Log;
using Entity = Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Models.Log
{
    public sealed class LogViewModel : IEntityLoadable<Entity>
    {
        public int Id { get; set; }

        public string Event { get; set; }

        public string CreatedAt { get; set; }

        public string CreatedAtDetail { get; set; }

        public object Data { get; set; }

        public string IpAddress { get; set; }

        public int UserId { get; set; }

        public UserLogViewModel User { get; set; }

        public void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Event = ((Events)entity.EventId).Translate();
            CreatedAt = entity.CreatedAt.ToHumanFmt();
            CreatedAtDetail = entity.CreatedAt.ToHumanDetailFmt();
            Data = entity.Data.TryParseJson(out var data) ? data : entity.Data;
            IpAddress = entity.IPAddress?.ToString();
            UserId = entity.UserId ?? 0;
        }
    }
}
