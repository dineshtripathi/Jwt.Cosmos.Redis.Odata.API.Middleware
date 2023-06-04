using System.Text.Json;
using JWTClaimsExtractor.Exceptions;
using Microsoft.AspNetCore.Http;

namespace JWTClaimsExtractor.Middleware;

public class JwtExceptionMiddleware
{
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
        context.Response.StatusCode = exception switch
        {
            UnauthorizedAccessException _ => StatusCodes.Status401Unauthorized,
            ArgumentNullException _ => StatusCodes.Status400BadRequest,
            HttpException httpException => httpException.StatusCode,
            TimeoutException _ => StatusCodes.Status504GatewayTimeout,
            _ => StatusCodes.Status500InternalServerError,
        };

        var result = JsonSerializer.Serialize(new { error = exception.Message });
        return context.Response.WriteAsync(result);
    }
}