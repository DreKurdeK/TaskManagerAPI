using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext); 
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred.");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var errorResponse = new ErrorResponse
            {
                StatusCode = 500,
                Message = "An unexpected error occurred.",
                Detail = ex.Message
            };
            await httpContext.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}