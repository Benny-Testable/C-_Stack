using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Services;

/// <summary>
/// Converts persistence entities into the response contracts exposed by the API.
/// </summary>
/// <remarks>
/// Centralised so that entity-to-contract projection exists exactly once per aggregate. This is
/// also the boundary that keeps internal columns out of payloads, supporting the Excel
/// "PII in API Response Count" expected value of zero unnecessary fields.
/// </remarks>
public static class ResponseMapper
{
    public static ScholarshipResponse ToResponse(this Scholarship scholarship)
    {
        ArgumentNullException.ThrowIfNull(scholarship);

        return new ScholarshipResponse
        {
            Id = scholarship.Id,
            Name = scholarship.Name,
            Description = scholarship.Description,
            SponsorName = scholarship.SponsorName,
            AwardAmount = scholarship.AwardAmount,
            TotalSlots = scholarship.TotalSlots,
            ApplicationOpensOn = scholarship.ApplicationOpensOn,
            ApplicationClosesOn = scholarship.ApplicationClosesOn,
            IsActive = scholarship.IsActive,
            CreatedAtUtc = scholarship.CreatedAtUtc,
            UpdatedAtUtc = scholarship.UpdatedAtUtc,
        };
    }

    public static ApplicantResponse ToResponse(this Applicant applicant)
    {
        ArgumentNullException.ThrowIfNull(applicant);

        return new ApplicantResponse
        {
            Id = applicant.Id,
            FullName = applicant.FullName,
            Email = applicant.Email,
            InstitutionName = applicant.InstitutionName,
            CreatedAtUtc = applicant.CreatedAtUtc,
            UpdatedAtUtc = applicant.UpdatedAtUtc,
        };
    }

    public static ApplicationResponse ToResponse(
        this ScholarshipApplication application,
        IApplicationStatusTransitionPolicy transitionPolicy)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(transitionPolicy);

        return new ApplicationResponse
        {
            Id = application.Id,
            ScholarshipId = application.ScholarshipId,
            ScholarshipName = application.Scholarship?.Name,
            ApplicantId = application.ApplicantId,
            Status = application.Status,
            StatusName = application.Status.ToString(),
            Motivation = application.Motivation,
            ReviewerNotes = application.ReviewerNotes,
            SubmittedAtUtc = application.SubmittedAtUtc,
            DecidedAtUtc = application.DecidedAtUtc,
            CreatedAtUtc = application.CreatedAtUtc,
            AllowedNextStatuses = transitionPolicy.AllowedTransitionsFrom(application.Status).ToArray(),
        };
    }
}
