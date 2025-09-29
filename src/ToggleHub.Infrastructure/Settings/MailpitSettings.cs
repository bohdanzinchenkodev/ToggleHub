namespace ToggleHub.Infrastructure.Settings;

public class MailpitSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}