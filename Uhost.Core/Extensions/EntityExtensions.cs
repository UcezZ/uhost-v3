using System.IO;
using Uhost.Core.Common;
using FileEntity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Extensions
{
    public static class EntityExtensions
    {
        public static string GetUrl(this FileEntity entity)
        {
            return Tools.UrlCombine(CoreSettings.PublicUrl, CoreSettings.UploadsUrl, entity.Token[0..2], entity.Token[2..4], entity.Token[4..], entity.Name);
        }

        public static string GetPath(this FileEntity entity)
        {
            return Path.GetFullPath(Path.Combine(CoreSettings.FileStoragePath, entity.Token[0..2], entity.Token[2..4], entity.Token[4..], entity.Name));
        }
    }
}
