﻿using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.File
{
    public class FileShortViewModel : BaseModel<Entity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int Size { get; set; }

        public string Mime { get; set; }

        public string Url { get; set; }

        internal string Path { get; set; }

        internal int? DynId { get; set; }

        internal string DynName { get; set; }

        internal Entity.Types TypeParsed => Type.ParseEnum<Entity.Types>() ?? Entity.Types.Other;

        public override void LoadFromEntity(Entity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Type = entity.Type;
            Size = entity.Size;
            Mime = entity.Mime;
            DynId = entity.DynId;
            DynName = entity.DynName;
            Url = entity.GetUrl();
            Path = entity.GetPath();
        }
    }
}
