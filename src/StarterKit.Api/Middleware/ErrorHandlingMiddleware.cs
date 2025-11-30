using System.Net;
using System.Text.Json;
using StarterKit.Application.Common.Exceptions;

namespace StarterKit.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError; // 500
        var message = "Internal Server Error";
        object? details = null;

        switch (exception)
        {
            // SPECIFIC exceptions must come FIRST
            case ValidationException valEx:
                code = HttpStatusCode.BadRequest; // 400
                message = "Validation Failed";
                details = valEx.Errors;
                break;
            
            // GENERAL exceptions come later
            case ApiException apiEx:
                code = (HttpStatusCode)apiEx.StatusCode;
                message = apiEx.Message;
                break;

            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized; // 401
                message = "Unauthorized";
                break;
                
            default:
                _logger.LogError(exception, "Unhandled Exception");
                break;
        }

        var response = new
        {
            code = (int)code,
            message,
            stack = _env.IsDevelopment() ? exception.StackTrace : null,
            details
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}