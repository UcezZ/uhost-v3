using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace Uhost.Core.Data.Entities
{
    [Table("Logs")]
    public class Log : BaseEntity
    {
        public enum Events : int
        {
            /// <summary>
            /// Неизвестный тип события
            /// </summary>
            Undefined = 0x00,

            ConsoleLoadDefaultData = 0x01,
            ConsoleCommandError = 0x02,

            FileUploaded = 0x11,
            FileUploadFail = 0x12,
            FileStoreFail = 0x13,

            UserAuth = 0x21,
            UserAuthFail = 0x22,
            UserRegisterQuery = 0x23,
            UserRegistered = 0x24,
            UserLogOut = 0x25,
            UserBanned = 0x26,
            UserUnbanned = 0x27,

            VideoUploaded = 0x31,
            VideoEdited = 0x32,
            VideoDeleted = 0x33,

            CommentPosted = 0x41,
            CommentDeleted = 0x42
        }

        public enum SortBy
        {
            Id,
            EventId,
            CreatedAt,
            IPAddress
        }

        [Column(TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; }

        public int EventId { get; set; }

        public int? UserId { get; set; }

        [Required, Column(TypeName = "jsonb")]
        public string Data { get; set; }

        [Column(TypeName = "inet")]
        public IPAddress IPAddress { get; set; }
    }
}
