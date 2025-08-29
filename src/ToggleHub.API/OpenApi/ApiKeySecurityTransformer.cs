using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OpenApi;

public class ApiKeySecurityTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // Define security scheme
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
        {
            Description = "API Key needed to access endpoints. Use: `Bearer {api_key}`",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKey"
        };

        // Apply requirement globally
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    }
}