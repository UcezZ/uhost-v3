using Microsoft.AspNetCore.Http;
using Uhost.Core.Attributes.Validation;

namespace Uhost.Core.Models.User
{
    public sealed class UserUpdateAvatarModel
    {
        /// <summary>
        /// Файл аватарки
        /// </summary>
        [FormFileValidation(ext: new[] { "jpg", "jfif", "jpeg", "jpe", "gif", "png", "bmp", "rle", "dib" })]
        public IFormFile File { get; set; }
    }
}
