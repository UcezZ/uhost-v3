using Sentry;
using System;
using System.Net.Mail;

namespace Uhost.Core.Services.Email
{
    public sealed class EmailService : IEmailService
    {
        private readonly SmtpClient _client;

        public EmailService()
        {
            _client = CoreSettings.SmtpConfig;
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
