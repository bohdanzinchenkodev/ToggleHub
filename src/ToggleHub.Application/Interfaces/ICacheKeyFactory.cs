namespace ToggleHub.Application.Interfaces;

public interface ICacheKeyFactory
{
    string Create(string template, params object[] parameters);
}