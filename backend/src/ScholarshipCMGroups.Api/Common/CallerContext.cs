using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Common;

/// <summary>
/// The authenticated identity on whose behalf a service operation runs.
/// </summary>
/// <remarks>
/// Passed explicitly from controllers into services so that ownership checks are part of the
/// service contract and can be unit-tested without an HTTP context. This is the mechanism behind
/// the Excel "BOLA Finding Count" expected value of zero cross-user resource access.
/// </remarks>
public readonly record struct CallerContext
{
    public CallerContext(UserRole role, int? applicantId)
    {
        Role = role;
        ApplicantId = applicantId;
    }

    public UserRole Role { get; }

    /// <summary>The applicant this caller owns, or <see langword="null"/> for administrators.</summary>
    public int? ApplicantId { get; }

    public bool IsAdministrator => Role == UserRole.Administrator;

    /// <summary>Returns <see langword="true"/> when the caller may act on <paramref name="applicantId"/>'s data.</summary>
    public bool CanAccessApplicant(int applicantId) => IsAdministrator || ApplicantId == applicantId;
}
