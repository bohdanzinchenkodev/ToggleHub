using System.Threading.Tasks;
using ToggleHub.Sdk.Models;

namespace ToggleHub.Sdk.Clients
{
    public interface IFlagsClient
    {
        /// <summary>
        /// Convenience generic that deserializes to a caller-provided type.
        /// </summary>
        Task<T> EvaluateAsync<T>(FlagEvaluationRequest request);
    }
}