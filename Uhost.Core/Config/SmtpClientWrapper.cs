using System.Net;
using System.Net.Mail;

namespace Uhost.Core.Config
{
    /// <summary>
    /// Обёртка для <see cref="SmtpClient"/> для нормальной десериализации.
    /// </summary>
    /// <remarks>
    /// При попытке десериализации, Newtonsoft не может найти реализацию <see cref="ICredentialsByHost"/> при указании <see cref="SmtpClient.Credentials"/>. В этом классе явно определяются используемые свойсива <see cref="SmtpClient"/>
    /// </remarks>
    public class SmtpClientWrapper
    {
        /// <inheritdoc cref="SmtpClient.Host"/>
        public string Host { get; set; }

        /// <inheritdoc cref="SmtpClient.Port"/>
        public int Port { get; set; }

        /// <inheritdoc cref="SmtpClient.Timeout"/>
        public int Timeout { get; set; }

        /// <inheritdoc cref="SmtpClient.EnableSsl"/>
        public bool EnableSsl { get; set; }

        /// <inheritdoc cref="SmtpClient.Credentials"/>
        public NetworkCredential Credentials { get; set; }

        public static implicit operator SmtpClient(SmtpClientWrapper client) =>
            new SmtpClient
            {
                Host = client.Host,
                Port = client.Port,
                Timeout = client.Timeout,
                EnableSsl = client.EnableSsl,
                Credentials = client.Credentials,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                DeliveryFormat = SmtpDeliveryFormat.International,
                UseDefaultCredentials = client.Credentials == null || string.IsNullOrWhiteSpace(client.Credentials.UserName) && string.IsNullOrWhiteSpace(client.Credentials.Password)
            };
    }
}
