using System.Security.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToggleHub.Domain.Exceptions;

namespace ToggleHub.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ApplicationException
                or UserCreationFailedException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException _ 
                or AuthenticationException _ => StatusCodes.Status401Unauthorized,
            NoAccessPermissionException _ => StatusCodes.Status403Forbidden,
            NotFoundException _ => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        httpContext.Response.StatusCode = statusCode;

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled server error at {Path}", httpContext.Request.Path);
        else
            _logger.LogWarning(exception, "Handled exception {ExceptionType} at {Path}", exception.GetType().Name, httpContext.Request.Path);

        var problemDetails = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Detail = statusCode == StatusCodes.Status500InternalServerError ? "An error occurred" : exception.Message,
                Title = "An error occurred",
                Status = statusCode
            }
        });

        return problemDetails;
    }
}