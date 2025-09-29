using ToggleHub.Domain.Entities;

namespace ToggleHub.Application.Interfaces;

public interface ISlugGenerator
{
    Task<string> GenerateAsync(
        string name,
        Func<string, Task<IEnumerable<string>>>? existingSlugsFactory = null
    );
}