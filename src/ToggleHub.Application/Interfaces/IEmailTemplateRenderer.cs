namespace ToggleHub.Application.Interfaces;

public interface IEmailTemplateRenderer
{
    Task<string> RenderTemplateAsync<T>(string templateName, T model);
}