using Newtonsoft.Json;
using static Uhost.Core.Data.Entities.Right;
using Entity = Uhost.Core.Data.Entities.Right;

namespace Uhost.Core.Models.Right
{
    public class RightViewModel : BaseModel<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public Rights RightCasted => (Rights)Id;

        public override void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
        }

        public override Entity FillEntity(Entity entity)
        {
            entity.Id = Id;
            entity.Name = Name;

            return entity;
        }
    }
}
