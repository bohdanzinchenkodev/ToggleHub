using SendGrid;
using SendGrid.Helpers.Mail;
using ToggleHub.Application.Interfaces;
using ToggleHub.Infrastructure.Settings;

namespace ToggleHub.Infrastructure.Email;

public class SendGridEmailSender : IEmailSender
{
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public SendGridEmailSender(SendGridSettings sendGridSettings)
    {
        _apiKey = sendGridSettings.ApiKey;
        _fromEmail = sendGridSettings.FromEmail;
        _fromName = sendGridSettings.FromName;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody, string? plainTextBody = null)
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_fromEmail, _fromName);
        var toAddr = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toAddr, subject, plainTextBody ?? " ", htmlBody);

        var response = await client.SendEmailAsync(msg);
        if ((int)response.StatusCode >= 400)
        {
            var error = await response.Body.ReadAsStringAsync();
            throw new InvalidOperationException($"SendGrid error: {response.StatusCode} - {error}");
        }
    }
}