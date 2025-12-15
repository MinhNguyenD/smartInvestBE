using notification.Dto;
using System.Net.Mail;

namespace notification.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;
        public EmailService(IConfiguration configuration) { 
            _configuration = configuration;
            var smtpSettings = _configuration.GetSection("Smtp");
            _smtpClient = new SmtpClient
            {
                Host = smtpSettings.GetValue<string>("Host"),
                Port = smtpSettings.GetValue<int>("Port"),
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(
                    smtpSettings.GetValue<string>("Username"),
                    smtpSettings.GetValue<string>("Password"))
            };
        }

        public void SendEmail(EmailMessage emailMessage)
        {
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(emailMessage.From),
                To = { new MailAddress(emailMessage.To) },
                Subject = emailMessage.Subject,
                Body = emailMessage.Body,
                IsBodyHtml = emailMessage.IsBodyHtml
            };
            _smtpClient.Send(mailMessage);
        }

    }
}
