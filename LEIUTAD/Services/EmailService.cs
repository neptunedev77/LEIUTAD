using System.Net.Mail;
using System.Net;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void SendEmail(string toEmail, string subject, string body)
    {
        var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_configuration["EmailSettings:SmtpPort"]),
            Credentials = new NetworkCredential(
                _configuration["EmailSettings:SenderEmail"],
                _configuration["EmailSettings:SenderPassword"]
            ),
            EnableSsl = true // Certifique-se de que o SSL está ativado
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["EmailSettings:SenderEmail"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true // Se quiser enviar emails com HTML
        };

        mailMessage.To.Add(toEmail);

        try
        {
            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao enviar o email", ex);
        }
    }
}
