using System.Text.Json;
using JWTClaimsExtractor.Exceptions;
using Microsoft.AspNetCore.Http;

namespace JWTClaimsExtractor.Middleware;

public class JwtExceptionMiddleware
{
    private static readonly Dictionary<Type, Func<Exception, (int StatusCode, string Message, string Details)>>
        ExceptionHandlers = new()
        {
            {
                typeof(UnauthorizedAccessException),
                ex => (StatusCodes.Status401Unauthorized, "Unauthorized access.", ex.Message)
            },
            {typeof(ArgumentNullException), ex => (StatusCodes.Status400BadRequest, "Bad request.", ex.Message)},
            {typeof(HttpException), ex => (((HttpException) ex).StatusCode, ex.Message, null)},
            {typeof(TimeoutException), ex => (StatusCodes.Status504GatewayTimeout, "Request timed out.", ex.Message)},
            {
                typeof(TokenValidationException), ex =>
                (
                    ((TokenValidationException) ex).StatusCode,
                    ((TokenValidationException) ex).Message,
                    ((TokenValidationException) ex).Details
                )
            }
        };

    private readonly RequestDelegate _next;


    public JwtExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var exceptionType = exception.GetType();
        var handler = ExceptionHandlers.ContainsKey(exceptionType)
            ? ExceptionHandlers[exceptionType]
            : ex => (StatusCodes.Status500InternalServerError, "An error occurred while processing your request.",
                ex.Message);


        var (statusCode, message, details) = handler(exception);

        context.Response.StatusCode = statusCode;

        var response = new
        {
            type = exceptionType.Name,
            status = statusCode,
            message,
            detailed = details ?? exception.Message
        };

        var result = JsonSerializer.Serialize(response);
        return context.Response.WriteAsync(result);
    }
}