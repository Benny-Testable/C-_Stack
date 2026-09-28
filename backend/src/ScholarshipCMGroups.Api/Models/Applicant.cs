namespace ScholarshipCMGroups.Api.Models;

/// <summary>
/// A person who applies for scholarships.
/// </summary>
/// <remarks>
/// Deliberately holds no national identifier, date of birth, or institution-issued student ID.
/// The Excel FERPA/COPPA metrics expect zero occurrences of those entity types anywhere in the
/// codebase, logs, or fixtures, so they are not modelled at all rather than modelled and masked.
/// </remarks>
public class Applicant
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? InstitutionName { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    public ICollection<ScholarshipApplication> Applications { get; } = new List<ScholarshipApplication>();
}
