using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Server.Services;

public class EmailSender(IConfiguration configuration) : IEmailSender
{
    private readonly string _smtpPort = configuration["Smtp:Port"] ??
                                        throw new Exception(
                                            "SMTP Port is not defined in appsettings.<environment>.json");

    private readonly string _host = configuration["Smtp:Host"] ??
                                    throw new Exception("SMTP Host is not defined in appsettings.<environment>.json");

    private readonly string _emailSender = configuration["Smtp:EmailSender"] ??
                                           throw new Exception(
                                               "SMTP Email sender is not defined in appsettings.<environment>.json");

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var client = new SmtpClient
        {
            Port = ushort.Parse(_smtpPort),
            Host = _host,
            EnableSsl = false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSender),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        await client.SendMailAsync(mailMessage);
    }
}