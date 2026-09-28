namespace ScholarshipCMGroups.Api.Models;

/// <summary>
/// A scholarship programme that applicants can apply to.
/// </summary>
public class Scholarship
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string SponsorName { get; set; } = string.Empty;

    public decimal AwardAmount { get; set; }

    public int TotalSlots { get; set; }

    public DateOnly ApplicationOpensOn { get; set; }

    public DateOnly ApplicationClosesOn { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public ICollection<ScholarshipApplication> Applications { get; } = new List<ScholarshipApplication>();

    /// <summary>
    /// Indicates whether <paramref name="onDate"/> falls inside the inclusive application window.
    /// </summary>
    public bool AcceptsApplicationsOn(DateOnly onDate) =>
        IsActive && onDate >= ApplicationOpensOn && onDate <= ApplicationClosesOn;
}
