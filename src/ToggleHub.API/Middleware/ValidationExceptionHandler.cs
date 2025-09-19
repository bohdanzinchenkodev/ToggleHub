
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ToggleHub.API.Middleware;

public class ValidationExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ValidationExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        var context = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Detail = "One or more validation errors occurred",
                Status = StatusCodes.Status400BadRequest
            }
        };

        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => LowercaseFirstCharEachProperty(g.Key),
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
        context.ProblemDetails.Extensions.Add("errors", errors);

        return await _problemDetailsService.TryWriteAsync(context);
    }
    private static string LowercaseFirstCharEachProperty(string key)
    {
        if (string.IsNullOrEmpty(key))
            return key;

        var parts = key.Split('.');

        for (int i = 0; i < parts.Length; i++)
        {
            // handle array indexes like RuleSets[0]
            var bracketIndex = parts[i].IndexOf('[');
            string propName = bracketIndex >= 0 ? parts[i].Substring(0, bracketIndex) : parts[i];
            string indexPart = bracketIndex >= 0 ? parts[i].Substring(bracketIndex) : "";

            if (!string.IsNullOrEmpty(propName))
            {
                parts[i] = char.ToLowerInvariant(propName[0]) + propName.Substring(1) + indexPart;
            }
        }

        return string.Join(".", parts);
    }
}