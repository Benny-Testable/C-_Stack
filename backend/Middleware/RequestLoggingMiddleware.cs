namespace ScholarshipCMGroups.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var started = DateTime.UtcNow;
        var path = context.Request.Path.Value ?? string.Empty;
        var method = context.Request.Method;
        _logger.LogInformation("Request started {Method} {Path} at {Started}", method, path, started);
        await _next(context);
        var elapsed = DateTime.UtcNow - started;
        _logger.LogInformation("Request finished {Method} {Path} status {Status} in {Elapsed} ms", method, path, context.Response.StatusCode, elapsed.TotalMilliseconds);
    }
}
