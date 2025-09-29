using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SendGrid.Helpers.Mail;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Settings;

namespace ToggleHub.Infrastructure.Email;

public class MailpitEmailSender(MailpitSettings mailpitSettings) : IEmailSender
{
    private readonly string _host = mailpitSettings.Host;
    private readonly int _port = mailpitSettings.Port;
    private readonly string _fromEmail = mailpitSettings.FromEmail;
    private readonly string _fromName = mailpitSettings.FromName;

    public async Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName, _fromEmail));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        var body = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = plainTextBody ?? " " // optional plain text
        };
        message.Body = body.ToMessageBody();

        using var client = new SmtpClient();
        // Mailpit does NOT use SSL; use None. For real SMTP, choose as needed.
        await client.ConnectAsync(_host, _port, SecureSocketOptions.None);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}