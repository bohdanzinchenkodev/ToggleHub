namespace ToggleHub.Application.Interfaces;

public interface ICacheKeyFormatter
{
    string Format(string template, params object[] parameters);
}