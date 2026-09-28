using System.ComponentModel.DataAnnotations;

namespace ScholarshipCMGroups.Api.Contracts;

/// <summary>
/// Payload for creating or replacing a scholarship programme.
/// </summary>
public record ScholarshipRequest : IValidatableObject
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; init; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; init; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string SponsorName { get; init; } = string.Empty;

    [Range(0.01, 100_000_000, ErrorMessage = "AwardAmount must be greater than zero.")]
    public decimal AwardAmount { get; init; }

    [Range(1, 100_000, ErrorMessage = "TotalSlots must be at least 1.")]
    public int TotalSlots { get; init; }

    [Required]
    public DateOnly ApplicationOpensOn { get; init; }

    [Required]
    public DateOnly ApplicationClosesOn { get; init; }

    public bool IsActive { get; init; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ApplicationClosesOn <= ApplicationOpensOn)
        {
            yield return new ValidationResult(
                "ApplicationClosesOn must be later than ApplicationOpensOn.",
                new[] { nameof(ApplicationClosesOn) });
        }
    }
}

/// <summary>
/// Scholarship programme as returned by the API.
/// </summary>
public record ScholarshipResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string SponsorName { get; init; } = string.Empty;

    public decimal AwardAmount { get; init; }

    public int TotalSlots { get; init; }

    public DateOnly ApplicationOpensOn { get; init; }

    public DateOnly ApplicationClosesOn { get; init; }

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAtUtc { get; init; }

    public DateTimeOffset? UpdatedAtUtc { get; init; }
}
