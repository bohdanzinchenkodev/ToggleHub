using System.Collections.Generic;

namespace ToggleHub.Sdk.Models
{
    public class FlagEvaluationRequest
    {
        public IDictionary<string, string> ConditionAttributes { get; set; } = new Dictionary<string, string>();
        public string UserId { get; set; } = string.Empty;
        public string FlagKey { get; set; } = string.Empty;
    }
}