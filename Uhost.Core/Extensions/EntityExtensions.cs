using System.IO;
using Uhost.Core.Common;
using FileEntity = Uhost.Core.Data.Entities.File;

namespace Uhost.Core.Extensions
{
    public static class EntityExtensions
    {
        public static string GetUrlPath(this FileEntity entity, FileEntity.Types? typeOverride = null)
        {
            var tk = entity.Token.Trim();
            var type = typeOverride ?? entity.Type.ParseEnum<FileEntity.Types>();

            return type switch
            {
                FileEntity.Types.Video240p or
                FileEntity.Types.Video360p or
                FileEntity.Types.Video480p or
                FileEntity.Types.Video720p or
                FileEntity.Types.Video1080p => Tools.UrlCombine(CoreSettings.VideosUrl, tk[0..2], tk[2..4], tk[4..]),
                _ => Tools.UrlCombine(CoreSettings.UploadsUrl, tk[0..2], tk[2..4], tk[4..], entity.Name),
            };
        }

        public static string GetUrl(this FileEntity entity, FileEntity.Types? typeOverride = null)
        {
            var tk = entity.Token.Trim();
            var type = typeOverride ?? entity.Type.ParseEnum<FileEntity.Types>();

            return type switch
            {
                FileEntity.Types.Video240p or
                FileEntity.Types.Video360p or
                FileEntity.Types.Video480p or
                FileEntity.Types.Video720p or
                FileEntity.Types.Video1080p => Tools.UrlCombine(CoreSettings.MediaServerUrl, CoreSettings.VideosUrl, tk[0..2], tk[2..4], tk[4..]),
                _ => Tools.UrlCombine(CoreSettings.PublicUrl, CoreSettings.UploadsUrl, tk[0..2], tk[2..4], tk[4..], entity.Name),
            };
        }

        public static string GetPath(this FileEntity entity, FileEntity.Types? typeOverride = null)
        {
            var tk = entity.Token.Trim();
            var type = typeOverride ?? entity.Type.ParseEnum<FileEntity.Types>();

            return type switch
            {
                FileEntity.Types.Video240p or
                FileEntity.Types.Video360p or
                FileEntity.Types.Video480p or
                FileEntity.Types.Video720p or
                FileEntity.Types.Video1080p => Path.GetFullPath(Path.Combine(CoreSettings.VideoStoragePath, tk[0..2], tk[2..4], tk[4..])),
                _ => Path.GetFullPath(Path.Combine(CoreSettings.FileStoragePath, tk[0..2], tk[2..4], tk[4..], entity.Name))
            };
        }
    }
}
