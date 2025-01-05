using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;

namespace LEIUTAD.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Biblioteca", emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                client.Connect(emailSettings["SmtpServer"], int.Parse(emailSettings["SmtpPort"]), false);
                client.Authenticate(emailSettings["SenderEmail"], emailSettings["SenderPassword"]);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
