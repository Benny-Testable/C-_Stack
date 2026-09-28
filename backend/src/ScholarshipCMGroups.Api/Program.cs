using System.Text.Json.Serialization;
using ScholarshipCMGroups.Api.Middleware;
using ScholarshipCMGroups.Api.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiOptions(builder.Configuration)
    .AddPersistence(builder.Configuration)
    .AddApplicationServices()
    .AddApiAuthentication(builder.Configuration)
    .AddApiRateLimiting(builder.Configuration)
    .AddApiCors(builder.Configuration)
    .AddApiDocumentation();

builder.Services
    .AddControllers()
    .AddJsonOptions(json => json.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();

var app = builder.Build();

// Ordering matters: security headers wrap every response including those produced by the
// exception handler, and rate limiting runs before authentication so that unauthenticated
// floods are shed before any cryptographic work happens.
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseCors(ServiceCollectionExtensions.CorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// The limiter registered as GlobalLimiter already covers every endpoint, so no per-route policy.
app.MapControllers();

await DatabaseInitializer.InitializeAsync(app.Services, CancellationToken.None).ConfigureAwait(false);

await app.RunAsync().ConfigureAwait(false);

/// <summary>
/// Entry point marker.
/// </summary>
/// <remarks>
/// Top-level statements compile into an <c>internal</c> <c>Program</c> class. Declaring the partial
/// explicitly makes it public so <c>WebApplicationFactory&lt;Program&gt;</c> in the test project can
/// reference it and host this exact pipeline. Nothing else is added to the type.
/// </remarks>
public partial class Program;
