namespace ToggleHub.Application.Interfaces;

public interface ICacheKeyRegistry
{
    void AddKey(string key);
    void RemoveKey(string key);
    IReadOnlyCollection<string> GetKeys();
    void Clear();
}