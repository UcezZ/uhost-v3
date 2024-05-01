using Sentry;
using System;
using System.Net.Mail;
using Uhost.Core.Extensions;
using Uhost.Core.Services.Log;
using static Uhost.Core.Data.Entities.Log;

namespace Uhost.Core.Services.Email
{
    public sealed class EmailService : IEmailService
    {
        private readonly ILogService _log;
        private readonly SmtpClient _client;

        public EmailService(ILogService log)
        {
            _client = CoreSettings.SmtpConfig;
            _log = log;
        }

        /// <summary>
        /// Отправка E-mail'а из объекта
        /// </summary>
        /// <param name="message"></param>
        public void Send(MailMessage message)
        {
            try
            {
                _client.Send(message);
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
                _log.Add(Events.EmailSendError, new
                {
                    Message = new
                    {
                        message.From,
                        message.To,
                        message.Subject,
                        message.IsBodyHtml,
                        message.Body,
                        AttachmentsCount = message.Attachments.Count
                    },
                    Exception = e.ToDetailedDataObject()
                });
                throw;
            }
        }

        /// <summary>
        /// Отправка E-mail'а из данных
        /// </summary>
        /// <param name="from">От кого</param>
        /// <param name="to">Кому</param>
        /// <param name="subject">Тема сообщения</param>
        /// <param name="body">Тело сообщения</param>
        /// <param name="isHtml">Тело это HTML</param>
        public void Send(string from, string to, string subject, string body, bool isHtml)
        {
            Send(new MailMessage(from, to) { Subject = subject, Body = body, IsBodyHtml = isHtml });
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
