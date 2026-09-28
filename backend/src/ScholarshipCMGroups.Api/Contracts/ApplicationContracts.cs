using System.ComponentModel.DataAnnotations;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Contracts;

/// <summary>
/// Payload for opening a draft application against a scholarship.
/// </summary>
public record ApplicationCreateRequest
{
    [Range(1, int.MaxValue)]
    public int ScholarshipId { get; init; }

    [Range(1, int.MaxValue)]
    public int ApplicantId { get; init; }

    [StringLength(2000)]
    public string? Motivation { get; init; }
}

/// <summary>
/// Payload for editing a draft application's motivation statement.
/// </summary>
public record ApplicationUpdateRequest
{
    [StringLength(2000)]
    public string? Motivation { get; init; }
}

/// <summary>
/// Payload for moving an application to a new lifecycle state.
/// </summary>
public record ApplicationTransitionRequest
{
    [Required]
    [EnumDataType(typeof(ApplicationStatus))]
    public ApplicationStatus TargetStatus { get; init; }

    [StringLength(2000)]
    public string? ReviewerNotes { get; init; }
}

/// <summary>
/// Application as returned by the API.
/// </summary>
public record ApplicationResponse
{
    public int Id { get; init; }

    public int ScholarshipId { get; init; }

    public string? ScholarshipName { get; init; }

    public int ApplicantId { get; init; }

    public ApplicationStatus Status { get; init; }

    public string StatusName { get; init; } = string.Empty;

    public string? Motivation { get; init; }

    public string? ReviewerNotes { get; init; }

    public DateTimeOffset? SubmittedAtUtc { get; init; }

    public DateTimeOffset? DecidedAtUtc { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public IReadOnlyCollection<ApplicationStatus> AllowedNextStatuses { get; init; } = Array.Empty<ApplicationStatus>();
}
