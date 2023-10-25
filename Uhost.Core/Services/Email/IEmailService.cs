using System.Net.Mail;

namespace Uhost.Core.Services.Email
{
    public interface IEmailService
    {
        void Send(MailMessage message);
        void Send(string from, string to, string subject, string body, bool isHtml);
    }
}
