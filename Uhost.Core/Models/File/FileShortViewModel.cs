using Newtonsoft.Json;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.File
{
    public class FileShortViewModel : IEntityLoadable<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public long Size { get; set; }

        public string Mime { get; set; }

        public string Url { get; set; }

        [JsonIgnore]
        public string Path { get; set; }

        [JsonIgnore]
        public int? DynId { get; set; }

        [JsonIgnore]
        public string DynName { get; set; }

        internal string UrlPath { get; set; }

        [JsonIgnore]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public bool Exists
        {
            get
            {
                try
                {
                    return System.IO.File.Exists(Path);
                }
                catch
                {
                    return false;
                }
            }
        }

        internal Entity.FileTypes TypeParsed => Type.ParseEnum<Entity.FileTypes>() ?? Entity.FileTypes.Other;

        public virtual void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Type = entity.Type;
            Size = entity.Size;
            Mime = entity.Mime;
            DynId = entity.DynId;
            DynName = entity.DynName;
            IsDeleted = entity.DeletedAt != null;
            Url = entity.GetUrl();
            Path = entity.GetPath();
            UrlPath = entity.GetUrlPath();
        }
    }
}
