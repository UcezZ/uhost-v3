using Microsoft.AspNetCore.Http;
using Uhost.Core.Attributes.Validation;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoCreateModel : VideoUpdateModel
    {
        internal int UserId { get; set; }

        /// <summary>
        /// Файл видео
        /// </summary>
        [FormFileValidation(maxFileSize: 1073741824, ext: new[] { "3g2", "3gp2", "3gp", "3gpp", "asf", "asr", "asx", "avi", "dvr", "flv", "IVF", "lsf", "lsx", "m1v", "m2ts", "m4v", "mov", "movie", "mp2", "mp4", "mp4v", "mpa", "mpe", "mpeg", "mpg", "mpv2", "nsc", "ogg", "ogv", "qt", "ts", "tts", "webm", "wm", "wmp", "wmv", "wmx", "wtv", "wvx" })]
        public IFormFile File { get; set; }

        public override Entity FillEntity(Entity entity)
        {
            base.FillEntity(entity);

            entity.UserId = UserId;

            return entity;
        }
    }
}
