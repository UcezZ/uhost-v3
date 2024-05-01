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
            EmailSendError = 0x03,

            FileUploaded = 0x11,
            FileUploadFail = 0x12,
            FileStoreFail = 0x13,
            FileDeleted = 0x14,
            FileDeletedPhysically = 0x15,

            UserAuth = 0x21,
            UserAuthFail = 0x22,
            UserRegisterQuery = 0x23,
            UserRegistered = 0x24,
            UserLogOut = 0x25,
            UserAvatarUploaded = 0x26,

            VideoUploaded = 0x31,
            VideoEdited = 0x32,
            VideoDeleted = 0x33,
            VideoConversionFailed = 0x34,
            VideoConversionCompleted = 0x35,
            VideoFetchFailed = 0x36,
            VideoFetchCompleted = 0x37,

            CommentPosted = 0x41,
            CommentDeleted = 0x42,

            SessionTerminated = 0x51
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
