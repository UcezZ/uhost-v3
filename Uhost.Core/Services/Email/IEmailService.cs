using System;
using System.Net.Mail;

namespace Uhost.Core.Services.Email
{
    public interface IEmailService : IDisposable
    {
        void Send(MailMessage message);
        void Send(string from, string to, string subject, string body, bool isHtml);
    }
}
