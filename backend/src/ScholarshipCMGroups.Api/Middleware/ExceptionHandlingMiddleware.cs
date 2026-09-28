using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Api.Common;

namespace ScholarshipCMGroups.Api.Middleware;

/// <summary>
/// Converts unhandled exceptions into RFC 7807 problem responses.
/// </summary>
/// <remarks>
/// The response body carries a status, a generic title, and a correlation id only. Exception
/// messages and stack traces are written to the log, never to the client, which is what the Excel
/// "Sensitive Information Tracking" metric checks for (expected value: zero sensitive data paths
/// reaching an insecure output).
/// </remarks>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await _next(context).ConfigureAwait(false);
        }
        catch (Exception exception) when (!context.Response.HasStarted)
        {
            var correlationId = Activity.Current?.Id ?? context.TraceIdentifier;

            LogMessages.UnhandledException(
                _logger,
                exception,
                context.Request.Method,
                context.Request.Path,
                correlationId);

            await WriteProblemAsync(context, correlationId).ConfigureAwait(false);
        }
    }

    private static async Task WriteProblemAsync(HttpContext context, string correlationId)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Instance = context.Request.Path,
        };
        problem.Extensions["correlationId"] = correlationId;

        context.Response.Clear();
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problem).ConfigureAwait(false);
    }
}
