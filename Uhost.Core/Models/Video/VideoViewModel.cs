using System;
using System.Collections.Generic;
using Entity = Uhost.Core.Data.Entities.Video;

namespace Uhost.Core.Models.Video
{
    public class VideoViewModel : VideoShortViewModel
    {
        /// <summary>
        /// Разрешить комментарии
        /// </summary>
        public bool AllowComments { get; set; }

        /// <summary>
        /// Разрешить реакции
        /// </summary>
        public bool AllowReactions { get; set; }

        /// <summary>
        /// URL для отдачи клиенту
        /// </summary>
        public IDictionary<string, string> Urls { get; set; }

        /// <summary>
        /// Размеры видеофайлов для скачивания
        /// </summary>
        public IDictionary<string, string> DownloadSizes { get; set; }

        /// <summary>
        /// Токен доступа для установки в куку
        /// </summary>
        internal string AccessToken { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Пути URL для генерации токенов
        /// </summary>
        internal IDictionary<string, string> UrlPaths { get; set; }

        internal TimeSpan CookieTtl { get; set; }

        public override void LoadFromEntity(Entity entity)
        {
            base.LoadFromEntity(entity);

            Description = entity.Description;
            AllowComments = entity.AllowComments;
            AllowReactions = entity.AllowReactions;
        }

        public string GetAccessToken() => AccessToken;

        public TimeSpan GetCookieTtl() => CookieTtl;
    }
}
