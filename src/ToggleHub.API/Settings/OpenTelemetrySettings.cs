using OpenTelemetry.Exporter;

namespace ToggleHub.API.Settings;

public class OpenTelemetrySettings
{
    public bool Enabled { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string ServiceVersion { get; set; } = string.Empty;
    public bool OtlpLogsEnabled { get; set; }
    public string OtlpEndpointLogs { get; set; } = string.Empty;
    public string OtlpProtocol { get; set; } = string.Empty;
    
    public bool OtlpTracesEnabled { get; set; }
    public string OtlpEndpointTraces { get; set; } = string.Empty;
    public bool OtlpMetricsEnabled { get; set; }
    public string OtlpEndpointMetrics { get; set; } = string.Empty;
    public  OtlpExportProtocol GetProtocol()
    {
        if (string.Equals(OtlpProtocol, "grpc", StringComparison.OrdinalIgnoreCase))
            return OtlpExportProtocol.Grpc;
        if (string.Equals(OtlpProtocol, "http", StringComparison.OrdinalIgnoreCase))
            return OtlpExportProtocol.HttpProtobuf; 
        
        throw new Exception("Invalid OpenTelemetry protocol configured. Supported protocols are 'grpc' and 'http'.");
    }
}