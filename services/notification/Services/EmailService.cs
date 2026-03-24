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
            _smtpClient = new SmtpClient
            {
                Host = _configuration["EMAILSERVER:HOST"]!,
                Port = int.Parse(_configuration["EMAILSERVER:PORT"]!),
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(
                    _configuration["EMAILSERVER:USERNAME"]!,
                    _configuration["EMAILSERVER:PASSWORD"]!)
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
