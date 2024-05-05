using Newtonsoft.Json;
using System;
using Uhost.Core.Common;
using Uhost.Core.Extensions;
using Uhost.Core.Models.User;
using Uhost.Core.Services.Token;

namespace Uhost.Core.Models.Session
{
    public sealed class SessionViewModel
    {
        /// <summary>
        /// Это текущая сессия
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// GUID сессии
        /// </summary>
        public string SessionGuid { get; set; }

        /// <summary>
        /// Когда истекает сессия
        /// </summary>
        public string ExpiresAt { get; set; }

        /// <summary>
        /// Через сколько истекает сессия
        /// </summary>
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Пользователь
        /// </summary>
        public UserShortViewModel User { get; set; }

        /// <summary>
        /// Инфо о клиенте
        /// </summary>
        public SessionClientInfoModel ClientInfo { get; set; }

        /// <summary>
        /// ИД пользователя
        /// </summary>
        [JsonIgnore]
        public int UserId { get; set; }

        internal SessionViewModel(RedisKeyInfo keyInfo)
        {
            var match = TokenService.RedisAuthKeyRegex.Match(keyInfo.RedisKey);

            if (match.Groups.Count > 2)
            {
                UserId = match.Groups[1].Value.ParseDigits();
                SessionGuid = match.Groups[2].Value.ToLower();
            }

            ExpiresIn = keyInfo.Expiry.ToHumanFmt();
            ExpiresAt = DateTime.Now.Add(keyInfo.Expiry).ToHumanFmt();
        }
    }
}
