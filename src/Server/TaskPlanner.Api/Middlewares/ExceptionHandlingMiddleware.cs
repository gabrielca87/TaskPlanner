using System.Net;
using System.Text.Json;
using TaskPlanner.Api.Common;
using TaskPlanner.Application.Exceptions;

namespace TaskPlanner.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = MapException(exception);

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception while processing request.");
        }

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Errors = errors
        };

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static (HttpStatusCode StatusCode, string Message, IReadOnlyDictionary<string, string[]>? Errors) MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => (HttpStatusCode.BadRequest, validationException.Message, validationException.Errors),
            UnauthorizedException unauthorizedException => (HttpStatusCode.Unauthorized, unauthorizedException.Message, null),
            ForbiddenException forbiddenException => (HttpStatusCode.Forbidden, forbiddenException.Message, null),
            NotFoundException notFoundException => (HttpStatusCode.NotFound, notFoundException.Message, null),
            ConflictException conflictException => (HttpStatusCode.Conflict, conflictException.Message, null),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
        };
    }
}
