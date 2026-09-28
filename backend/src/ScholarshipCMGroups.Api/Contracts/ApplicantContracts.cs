using System.ComponentModel.DataAnnotations;

namespace ScholarshipCMGroups.Api.Contracts;

/// <summary>
/// Payload for creating or updating an applicant profile.
/// </summary>
public record ApplicantRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [StringLength(200)]
    public string? InstitutionName { get; init; }
}

/// <summary>
/// Applicant profile as returned by the API.
/// </summary>
public record ApplicantResponse
{
    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string? InstitutionName { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}
