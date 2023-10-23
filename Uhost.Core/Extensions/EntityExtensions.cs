using System.IO;
using Uhost.Core.Common;
using FileEntity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Extensions
{
    public static class EntityExtensions
    {
        public static string GetUrl(this FileEntity entity)
        {
            var tk = entity.Token.Trim();

            return Tools.UrlCombine(CoreSettings.PublicUrl, CoreSettings.UploadsUrl, tk[0..2], tk[2..4], tk[4..], entity.Name);
        }

        public static string GetPath(this FileEntity entity)
        {
            var tk = entity.Token.Trim();

            return Path.GetFullPath(Path.Combine(CoreSettings.FileStoragePath, tk[0..2], tk[2..4], tk[4..], entity.Name));
        }
    }
}
