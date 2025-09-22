using RazorLight;
using ToggleHub.Application.Interfaces;

namespace ToggleHub.Infrastructure.Email;

public class RazorEmailTemplateRenderer : IEmailTemplateRenderer
{
    private readonly RazorLightEngine _engine;

    public RazorEmailTemplateRenderer()
    {
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(Path.Combine(AppContext.BaseDirectory, "Templates"))
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderTemplateAsync<T>(string templateName, T model)
    {
        var path = $"{templateName}.cshtml";
        return await _engine.CompileRenderAsync(path, model);
    }
}