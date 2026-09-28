using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ScholarshipCMGroups.Api.Controllers;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Api;

/// <summary>
/// Liveness probe, security headers, and the published OpenAPI document.
/// </summary>
public sealed class HealthAndDocumentationTests : IClassFixture<ScholarshipApiFactory>
{
    private readonly ScholarshipApiFactory _factory;

    public HealthAndDocumentationTests(ScholarshipApiFactory factory) => _factory = factory;

    [Fact]
    public async Task The_health_endpoint_is_reachable_without_a_token()
    {
        // Backs the Excel "API Uptime %" metric, which is derived from successful health pings.
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri("/api/health", UriKind.Relative));
        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>(ApiClientExtensions.Json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", payload!.Status);
    }

    [Theory]
    [InlineData("X-Content-Type-Options", "nosniff")]
    [InlineData("X-Frame-Options", "DENY")]
    [InlineData("Referrer-Policy", "no-referrer")]
    [InlineData("Cross-Origin-Resource-Policy", "same-origin")]
    public async Task Every_response_carries_the_expected_security_header(string header, string expectedValue)
    {
        // Evidence for the Excel "Security Header Presence" family. Asserted on a real response so
        // the middleware order is verified, not just the middleware's existence.
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri("/api/health", UriKind.Relative));

        Assert.True(response.Headers.Contains(header), $"{header} is missing.");
        Assert.Equal(expectedValue, Assert.Single(response.Headers.GetValues(header)));
    }

    [Fact]
    public async Task A_content_security_policy_is_sent()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri("/api/health", UriKind.Relative));
        var policy = Assert.Single(response.Headers.GetValues("Content-Security-Policy"));

        Assert.Contains("default-src 'none'", policy, StringComparison.Ordinal);
        Assert.Contains("frame-ancestors 'none'", policy, StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_server_header_is_not_disclosed()
    {
        // Suppresses the version banner the Excel "Information Disclosure" metric looks for.
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri("/api/health", UriKind.Relative));

        Assert.DoesNotContain("Server", response.Headers.Select(h => h.Key), StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task The_OpenAPI_document_describes_every_controller_route()
    {
        // The document is the artifact the Excel "API Contract Conformance" metric reads, so this
        // test both proves it is generated and pins the route surface it advertises.
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri("/swagger/v1/swagger.json", UriKind.Relative));
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var paths = document.RootElement.GetProperty("paths");

        foreach (var route in new[]
                 {
                     "/api/health",
                     "/api/auth/register",
                     "/api/auth/login",
                     "/api/scholarships",
                     "/api/scholarships/{id}",
                     "/api/applicants",
                     "/api/applicants/{id}",
                     "/api/applications",
                     "/api/applications/{id}",
                     "/api/applications/{id}/transitions",
                 })
        {
            Assert.True(paths.TryGetProperty(route, out _), $"{route} is missing from the OpenAPI document.");
        }
    }

    [Fact]
    public async Task The_OpenAPI_document_declares_the_bearer_security_scheme()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri("/swagger/v1/swagger.json", UriKind.Relative));
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        var scheme = document.RootElement
            .GetProperty("components")
            .GetProperty("securitySchemes")
            .GetProperty("Bearer");

        Assert.Equal("http", scheme.GetProperty("type").GetString());
        Assert.Equal("bearer", scheme.GetProperty("scheme").GetString());
    }

    [Fact]
    public async Task An_unknown_route_returns_404_without_a_stack_trace()
    {
        using var client = _factory.CreateClient();

        var response = await client.GetAsync(new Uri("/api/does-not-exist", UriKind.Relative));
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.DoesNotContain("at ScholarshipCMGroups", body, StringComparison.Ordinal);
    }
}
