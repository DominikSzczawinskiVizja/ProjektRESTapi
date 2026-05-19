using api.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace api.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Couldn't find: {Message}. Path: {Path}", ex.Message, context.Request.Path);

                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Invalid operation: {Message}. Path: {Path}", ex.Message, context.Request.Path);

                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Unauthorized Access: {Message}. Path: {Path}", ex.Message, context.Request.Path);

                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                _logger.LogWarning("Forbidden: {Message}. Path: {Path}", ex.Message, context.Request.Path);

                context.Response.StatusCode = 403;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Internal Server Error. Path: {Path}", context.Request.Path);

                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Internal server error" });
            }
        }
    }

}
