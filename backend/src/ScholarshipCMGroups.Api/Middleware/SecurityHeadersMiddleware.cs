namespace ScholarshipCMGroups.Api.Middleware;

/// <summary>
/// Adds the response headers required by the Excel "Missing Security Header Count" metric.
/// </summary>
/// <remarks>
/// The metric's expected value is zero responses missing
/// <c>Strict-Transport-Security</c>, <c>X-Frame-Options</c>, <c>Content-Security-Policy</c>, or
/// <c>X-Content-Type-Options</c>. HSTS is emitted only over HTTPS, as required by RFC 6797.
/// </remarks>
public sealed class SecurityHeadersMiddleware
{
    /// <summary>
    /// The API returns JSON only, so the policy denies every content source outright. Swagger UI is
    /// served under its own relaxed policy by <c>UseSwaggerUI</c> in Development.
    /// </summary>
    private const string ContentSecurityPolicy =
        "default-src 'none'; frame-ancestors 'none'; base-uri 'none'; form-action 'none'";

    private const string StrictTransportSecurity = "max-age=31536000; includeSubDomains";

    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Response.OnStarting(() =>
        {
            ApplyHeaders(context);
            return Task.CompletedTask;
        });

        return _next(context);
    }

    private static void ApplyHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Content-Security-Policy"] = ContentSecurityPolicy;
        headers["Referrer-Policy"] = "no-referrer";
        headers["Cross-Origin-Resource-Policy"] = "same-origin";

        if (context.Request.IsHttps)
        {
            headers["Strict-Transport-Security"] = StrictTransportSecurity;
        }

        // Removes the framework banner so the response does not advertise the server stack.
        headers.Remove("Server");
    }
}
