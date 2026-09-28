using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Api.Common;

namespace ScholarshipCMGroups.Api.Controllers;

/// <summary>Liveness probe.</summary>
/// <remarks>
/// The Excel "API Uptime %" metric is derived from successful health-check pings, so this endpoint
/// is the artifact that metric reads. It reports liveness only and exposes no build or host detail.
/// </remarks>
[Route("api/health")]
[AllowAnonymous]
public sealed class HealthController : ApiControllerBase
{
    private readonly IClock _clock;

    public HealthController(IClock clock)
    {
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <summary>Returns 200 while the API is able to serve requests.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> Get() =>
        Ok(new HealthResponse { Status = "Healthy", CheckedAtUtc = _clock.UtcNow });
}

/// <summary>Health probe payload.</summary>
public record HealthResponse
{
    public string Status { get; init; } = string.Empty;

    public DateTimeOffset CheckedAtUtc { get; init; }
}
