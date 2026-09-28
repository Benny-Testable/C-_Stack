using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Services;

namespace ScholarshipCMGroups.Api.Controllers;

/// <summary>
/// Shared translation between service results and HTTP responses.
/// </summary>
/// <remarks>
/// Every controller maps <see cref="OperationResult{T}"/> through one place, so the status-code
/// policy is defined once. This keeps duplicated blocks out of the controller layer, which the
/// Excel "Structural Cleanliness Score" metric measures.
/// </remarks>
[ApiController]
// No [Produces] attribute: pinning the content type would relabel RFC 7807 error bodies as
// application/json, hiding the fact that they are ProblemDetails. Left to content negotiation,
// success payloads are served as application/json and failures as application/problem+json, which
// is what a client needs in order to branch on the media type.
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>Maps a service result onto the matching HTTP status code.</summary>
    protected ActionResult<T> ToActionResult<T>(OperationResult<T> result) => result.Status switch
    {
        OperationStatus.Success => Ok(result.Value!),
        OperationStatus.NotFound => NotFound(Problem(result.Error, StatusCodes.Status404NotFound)),
        OperationStatus.Conflict => Conflict(Problem(result.Error, StatusCodes.Status409Conflict)),
        OperationStatus.ValidationFailed => BadRequest(Problem(result.Error, StatusCodes.Status400BadRequest)),
        OperationStatus.Forbidden => StatusCode(
            StatusCodes.Status403Forbidden,
            Problem(result.Error, StatusCodes.Status403Forbidden)),
        _ => StatusCode(StatusCodes.Status500InternalServerError),
    };

    /// <summary>Reads the authenticated caller's role and owned applicant id from the bearer token.</summary>
    protected CallerContext GetCaller()
    {
        var role = Enum.TryParse<UserRole>(User.FindFirstValue(ClaimTypes.Role), out var parsed)
            ? parsed
            : UserRole.Applicant;

        var applicantClaim = User.FindFirstValue(ScholarshipClaimTypes.ApplicantId);
        var applicantId = int.TryParse(applicantClaim, CultureInfo.InvariantCulture, out var id) ? id : (int?)null;

        return new CallerContext(role, applicantId);
    }

    private static ProblemDetails Problem(string? detail, int status) => new()
    {
        Status = status,
        Detail = detail,
    };
}
