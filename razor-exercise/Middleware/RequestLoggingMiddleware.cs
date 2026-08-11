using System.Diagnostics;

namespace razor_exercise.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }

        // finally is used to ensure cleanup code is executed regardless of whether an exception is thrown or not. 
        // In this case, it ensures that the stopwatch is stopped and the log entry is created even if an 
        // exception occurs during the request processing.
        finally
        {
            stopwatch.Stop();

            _logger.LogInformation(
                "{Method} {Path} returned {StatusCode} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
