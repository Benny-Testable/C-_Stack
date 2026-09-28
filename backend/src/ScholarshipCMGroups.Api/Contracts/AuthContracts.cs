using System.ComponentModel.DataAnnotations;

namespace ScholarshipCMGroups.Api.Contracts;

/// <summary>
/// Payload for self-registering an applicant account.
/// </summary>
public record RegisterRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Minimum length is enforced by <c>Authentication:MinimumPasswordLength</c> rather than by an
    /// attribute, because the workbook states no password policy and the value must stay
    /// configurable. See docs/clarifications.md item C-04.
    /// </summary>
    [Required]
    [StringLength(256)]
    public string Password { get; init; } = string.Empty;

    [StringLength(200)]
    public string? InstitutionName { get; init; }
}

/// <summary>
/// Payload for exchanging credentials for an access token.
/// </summary>
public record LoginRequest
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(256)]
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Issued access token and the identity it represents.
/// </summary>
public record AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAtUtc { get; init; }

    public string Role { get; init; } = string.Empty;

    public int? ApplicantId { get; init; }
}
