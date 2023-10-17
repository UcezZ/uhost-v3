using Microsoft.AspNetCore.Http;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Extensions;
using Entity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Models.File
{
    public class FileUploadModel
    {
        /// <summary>
        /// Файл
        /// </summary>
        [FormFileValidation(maxFileSize: 0x40000000)]
        public IFormFile File { get; set; }

        /// <summary>
        /// Тип файла
        /// </summary>
        /// <example>Other</example>
        [EnumValidation(typeof(Entity.Types), nameof(Entity.Types.Other))]
        public string Type { get; set; }

        /// <summary>
        /// ИД динамической сущности
        /// </summary>
        public int? DynId { get; set; }

        /// <summary>
        /// Имя динамической сущности
        /// </summary>
        [EntityNameValidation(true)]
        public string DynName { get; set; }

        internal Entity.Types? TypeParsed => Type.ParseEnum<Entity.Types>();
    }
}
