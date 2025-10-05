namespace ToggleHub.API.Settings;

public class OpenTelemetrySettings
{
    public bool Enabled { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceVersion { get; set; } = string.Empty;
    public string OtlpEndpoint { get; set; } = string.Empty;
}