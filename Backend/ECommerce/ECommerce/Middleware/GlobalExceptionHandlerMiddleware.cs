using ECommerce.Utils;
using System.Net;
using System.Text.Json;

namespace ECommerce.Middleware
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Log the error to console
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff}");
            Console.WriteLine($"Exception Type: {exception.GetType().Name}");
            Console.WriteLine($"Message: {exception.Message}");
            Console.WriteLine($"Stack Trace: {exception.StackTrace}");
            Console.ResetColor();

            var response = new ApiError(
                statusCode: (int)HttpStatusCode.InternalServerError,
                message: "Internal Server Error",
                errors: new List<string> { "An unexpected error occurred. Please try again later." }
            );

            var jsonResponse = JsonSerializer.Serialize(response);
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
