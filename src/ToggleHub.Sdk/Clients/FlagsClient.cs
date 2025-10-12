using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ToggleHub.Sdk.Models;
using ToggleHub.Sdk.Options;

namespace ToggleHub.Sdk.Clients
{
    public class FlagsClient : IFlagsClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
        
        private readonly HttpClient _http;
        private readonly ToggleHubClientOptions _options;

        public FlagsClient(ToggleHubClientOptions options, HttpClient http)
        {
            _options = options;
            _http = http;
        }
        
        public async Task<T> EvaluateAsync<T>(FlagEvaluationRequest request)
        {
            using(var doc = await EvaluateAsync(request).ConfigureAwait(false))
            {
                return doc.Deserialize<T>(JsonOptions);
            }
        }
        
        private async Task<JsonDocument> EvaluateAsync(FlagEvaluationRequest request)
        {
            using (var msg = new HttpRequestMessage(HttpMethod.Post, "/api/flags/evaluate"))
            {
                msg.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_options.ApiKey}");

                var json = JsonSerializer.Serialize(request, JsonOptions);
                msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await _http.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
                {
                    resp.EnsureSuccessStatusCode();
                    using (var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        return await JsonDocument.ParseAsync(stream).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}