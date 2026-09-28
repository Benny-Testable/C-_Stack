namespace ScholarshipCMGroups.Api.Models;

/// <summary>
/// One applicant's application to one scholarship.
/// </summary>
public class ScholarshipApplication
{
    public int Id { get; set; }

    public int ScholarshipId { get; set; }

    public Scholarship? Scholarship { get; set; }

    public int ApplicantId { get; set; }

    public Applicant? Applicant { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;

    public string? Motivation { get; set; }

    public string? ReviewerNotes { get; set; }

    public DateTimeOffset? SubmittedAtUtc { get; set; }

    public DateTimeOffset? DecidedAtUtc { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? UpdatedAtUtc { get; set; }

    /// <summary>Optimistic concurrency token, mapped to a SQL Server <c>rowversion</c> column.</summary>
    public byte[]? RowVersion { get; set; }
}
