using System.Net;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Api;

/// <summary>
/// Proves the API sheds load once the configured request budget is spent.
/// </summary>
/// <remarks>
/// The Excel metric "APIs Without Rate Limiting Count" expects zero endpoints to be unlimited, and
/// expects a 429 once the threshold is crossed. The workbook states neither a request count nor a
/// window, so the limit is configuration-driven — see docs/clarifications.md item C-05. This test
/// therefore asserts the mechanism against a deliberately small limit rather than asserting a
/// specific production threshold the workbook does not define.
/// </remarks>
public sealed class RateLimitingApiTests
{
    private const int PermitLimit = 5;

    [Fact]
    public async Task Requests_beyond_the_configured_limit_are_answered_with_429()
    {
        using var factory = new ScholarshipApiFactory(PermitLimit);
        using var client = factory.CreateClient();
        var health = new Uri("/api/health", UriKind.Relative);

        var allowed = new List<HttpStatusCode>();
        for (var attempt = 0; attempt < PermitLimit; attempt++)
        {
            allowed.Add((await client.GetAsync(health)).StatusCode);
        }

        var overLimit = await client.GetAsync(health);

        Assert.All(allowed, status => Assert.Equal(HttpStatusCode.OK, status));
        Assert.Equal(HttpStatusCode.TooManyRequests, overLimit.StatusCode);
    }

    [Fact]
    public async Task The_limiter_also_covers_the_authentication_endpoints()
    {
        // Login is the endpoint most worth protecting, because an unlimited one enables credential
        // stuffing. The global limiter must reach it, not just the resource controllers.
        using var factory = new ScholarshipApiFactory(PermitLimit);
        using var client = factory.CreateClient();
        var health = new Uri("/api/health", UriKind.Relative);

        for (var attempt = 0; attempt < PermitLimit; attempt++)
        {
            await client.GetAsync(health);
        }

        var response = await client.PostAsync(new Uri("/api/auth/login", UriKind.Relative), content: null);

        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
    }
}
