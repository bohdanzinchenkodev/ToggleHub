namespace ToggleHub.Sdk.Options
{
    public class ToggleHubClientOptions
    {
        /// <summary>Base URL of your ToggleHub API, e.g. https://api.toggle-hub.com</summary>
        public string BaseAddress { get; set; } = "http://api.toggle-hub.com/";

        /// <summary>API key value to send on each request.</summary>
        public string ApiKey { get; set; } = string.Empty;
        
        /// <summary>Request timeout in seconds for the underlying HttpClient.</summary>
        public int TimeoutSeconds { get; set; } = 10;
    }
}