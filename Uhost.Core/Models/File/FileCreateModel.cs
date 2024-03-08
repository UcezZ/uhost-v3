using Uhost.Core.Common;
using Entity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.File
{
    /// <summary>
    /// Модель для внутреннего использования, не для API
    /// </summary>
    public class FileCreateModel : BaseModel<Entity>
    {
        public Entity.FileTypes Type { get; set; }
        public string Name { get; set; }
        public int? UserId { get; set; }
        public int Size { get; set; }
        public string Mime { get; set; }
        public int? DynId { get; set; }
        public string DynName { get; set; }
        public string Digest { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            entity.Type = Type.ToString();
            entity.Name = Name ?? string.Empty;
            entity.UserId = UserId;
            entity.Size = Size;
            entity.Mime = Tools.GetMimeFromName(entity.Name);
            entity.DynId = DynId;
            entity.DynName = DynName;
            entity.Digest = Digest ?? string.Empty;

            return entity;
        }
    }
}
