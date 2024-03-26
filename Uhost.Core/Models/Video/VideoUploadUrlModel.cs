using System;
using Uhost.Core.Attributes.Validation;
using Uhost.Core.Properties;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoUploadUrlModel : VideoUpdateModel
    {
        internal int UserId { get; set; }

        /// <summary>
        /// Максимальная продолжительность видео
        /// </summary>
        [TimeValidation(3, 60 * 60 * 4, false)]
        public string MaxDuration { get; set; }

        internal TimeSpan? MaxDurationParsed => TimeSpan.TryParse(MaxDuration, out var parsed) ? parsed : null;

        /// <summary>
        /// Ссылка на видео
        /// </summary>
        [RegExpValidation(@"^(ftps?|https?|rtsp|rtmp|udp):\/\/\S+$", ErrorMessageResourceType = typeof(CoreStrings), ErrorMessageResourceName = nameof(CoreStrings.Common_Error_Invalid))]
        public string Url { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.UserId = UserId;

            return entity;
        }
    }
}
