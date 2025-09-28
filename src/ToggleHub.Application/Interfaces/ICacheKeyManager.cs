namespace ToggleHub.Application.Interfaces;

public interface ICacheKeyManager
{
    void AddKey(string key);
    void RemoveKey(string key);
    IReadOnlyCollection<string> GetKeys();
    void Clear();
}