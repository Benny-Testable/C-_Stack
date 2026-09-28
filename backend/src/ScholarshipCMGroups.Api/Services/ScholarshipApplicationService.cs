using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Repositories;

namespace ScholarshipCMGroups.Api.Services;

/// <summary>
/// Application logic for the scholarship application lifecycle.
/// </summary>
public interface IScholarshipApplicationService
{
    Task<PagedResult<ApplicationResponse>> GetPageAsync(
        CallerContext caller,
        int? page,
        int? pageSize,
        int? scholarshipId,
        ApplicationStatus? status,
        CancellationToken cancellationToken);

    Task<OperationResult<ApplicationResponse>> GetByIdAsync(
        CallerContext caller,
        int id,
        CancellationToken cancellationToken);

    Task<OperationResult<ApplicationResponse>> CreateDraftAsync(
        CallerContext caller,
        ApplicationCreateRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ApplicationResponse>> UpdateDraftAsync(
        CallerContext caller,
        int id,
        ApplicationUpdateRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ApplicationResponse>> TransitionAsync(
        CallerContext caller,
        int id,
        ApplicationTransitionRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ScholarshipStatisticsResponse>> GetStatisticsAsync(
        int scholarshipId,
        CancellationToken cancellationToken);
}

/// <summary>
/// Descriptive counts for one scholarship. Reports observed state only; it applies no quota rule,
/// because the workbook defines none. See docs/clarifications.md item C-06.
/// </summary>
public record ScholarshipStatisticsResponse
{
    public int ScholarshipId { get; init; }

    public int TotalSlots { get; init; }

    public int SubmittedCount { get; init; }

    public int UnderReviewCount { get; init; }

    public int ApprovedCount { get; init; }

    public int RejectedCount { get; init; }

    public int RemainingSlots => Math.Max(0, TotalSlots - ApprovedCount);
}

/// <inheritdoc cref="IScholarshipApplicationService"/>
public sealed class ScholarshipApplicationService : IScholarshipApplicationService
{
    /// <summary>
    /// Target states an administrator alone may set. PROVISIONAL — see docs/clarifications.md C-03.
    /// </summary>
    private static readonly ApplicationStatus[] AdministratorOnlyTargets =
    {
        ApplicationStatus.UnderReview,
        ApplicationStatus.Approved,
        ApplicationStatus.Rejected,
    };

    private readonly IScholarshipApplicationRepository _applications;
    private readonly IScholarshipRepository _scholarships;
    private readonly IApplicantRepository _applicants;
    private readonly IApplicationStatusTransitionPolicy _transitionPolicy;
    private readonly IClock _clock;

    public ScholarshipApplicationService(
        IScholarshipApplicationRepository applications,
        IScholarshipRepository scholarships,
        IApplicantRepository applicants,
        IApplicationStatusTransitionPolicy transitionPolicy,
        IClock clock)
    {
        _applications = applications ?? throw new ArgumentNullException(nameof(applications));
        _scholarships = scholarships ?? throw new ArgumentNullException(nameof(scholarships));
        _applicants = applicants ?? throw new ArgumentNullException(nameof(applicants));
        _transitionPolicy = transitionPolicy ?? throw new ArgumentNullException(nameof(transitionPolicy));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public async Task<PagedResult<ApplicationResponse>> GetPageAsync(
        CallerContext caller,
        int? page,
        int? pageSize,
        int? scholarshipId,
        ApplicationStatus? status,
        CancellationToken cancellationToken)
    {
        var pageRequest = new PageRequest(page, pageSize);

        // Non-administrators are pinned to their own applicant id at the query level, so a
        // hand-crafted filter cannot widen the result set.
        var applicantFilter = caller.IsAdministrator ? null : caller.ApplicantId;

        var (items, totalCount) = await _applications
            .GetPageAsync(pageRequest, applicantFilter, scholarshipId, status, cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<ApplicationResponse>
        {
            Items = items.Select(a => a.ToResponse(_transitionPolicy)).ToArray(),
            Page = pageRequest.Page,
            PageSize = pageRequest.PageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<OperationResult<ApplicationResponse>> GetByIdAsync(
        CallerContext caller,
        int id,
        CancellationToken cancellationToken)
    {
        var application = await _applications.GetWithScholarshipAsync(id, cancellationToken).ConfigureAwait(false);
        if (application is null)
        {
            return OperationResult.NotFound<ApplicationResponse>($"Application {id} was not found.");
        }

        if (!caller.CanAccessApplicant(application.ApplicantId))
        {
            return OperationResult.Forbidden<ApplicationResponse>("The application belongs to another applicant.");
        }

        return OperationResult.Success(application.ToResponse(_transitionPolicy));
    }

    public async Task<OperationResult<ApplicationResponse>> CreateDraftAsync(
        CallerContext caller,
        ApplicationCreateRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!caller.CanAccessApplicant(request.ApplicantId))
        {
            return OperationResult.Forbidden<ApplicationResponse>(
                "An application can only be created for the authenticated applicant.");
        }

        var scholarship = await _scholarships.GetByIdAsync(request.ScholarshipId, cancellationToken).ConfigureAwait(false);
        if (scholarship is null)
        {
            return OperationResult.NotFound<ApplicationResponse>(
                $"Scholarship {request.ScholarshipId} was not found.");
        }

        var applicantExists = await _applicants.GetByIdAsync(request.ApplicantId, cancellationToken).ConfigureAwait(false);
        if (applicantExists is null)
        {
            return OperationResult.NotFound<ApplicationResponse>($"Applicant {request.ApplicantId} was not found.");
        }

        if (!scholarship.AcceptsApplicationsOn(_clock.Today))
        {
            return OperationResult.ValidationFailed<ApplicationResponse>(
                "The scholarship is not accepting applications today.");
        }

        var duplicate = await _applications
            .ExistsForApplicantAsync(request.ScholarshipId, request.ApplicantId, cancellationToken)
            .ConfigureAwait(false);
        if (duplicate)
        {
            return OperationResult.Conflict<ApplicationResponse>(
                "This applicant already has an application for the scholarship.");
        }

        var application = new ScholarshipApplication
        {
            ScholarshipId = request.ScholarshipId,
            ApplicantId = request.ApplicantId,
            Status = ApplicationStatus.Draft,
            Motivation = request.Motivation?.Trim(),
            CreatedAtUtc = _clock.UtcNow,
            Scholarship = scholarship,
        };

        await _applications.AddAsync(application, cancellationToken).ConfigureAwait(false);
        await _applications.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(application.ToResponse(_transitionPolicy));
    }

    public async Task<OperationResult<ApplicationResponse>> UpdateDraftAsync(
        CallerContext caller,
        int id,
        ApplicationUpdateRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var application = await _applications.GetWithScholarshipAsync(id, cancellationToken).ConfigureAwait(false);
        if (application is null)
        {
            return OperationResult.NotFound<ApplicationResponse>($"Application {id} was not found.");
        }

        if (!caller.CanAccessApplicant(application.ApplicantId))
        {
            return OperationResult.Forbidden<ApplicationResponse>("The application belongs to another applicant.");
        }

        if (application.Status != ApplicationStatus.Draft)
        {
            return OperationResult.ValidationFailed<ApplicationResponse>(
                "Only a draft application can be edited.");
        }

        application.Motivation = request.Motivation?.Trim();
        application.UpdatedAtUtc = _clock.UtcNow;

        _applications.Update(application);
        await _applications.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(application.ToResponse(_transitionPolicy));
    }

    public async Task<OperationResult<ApplicationResponse>> TransitionAsync(
        CallerContext caller,
        int id,
        ApplicationTransitionRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var application = await _applications.GetWithScholarshipAsync(id, cancellationToken).ConfigureAwait(false);
        if (application is null)
        {
            return OperationResult.NotFound<ApplicationResponse>($"Application {id} was not found.");
        }

        var authorisation = AuthoriseTransition(caller, application, request.TargetStatus);
        if (authorisation is not null)
        {
            return authorisation.Value;
        }

        if (!_transitionPolicy.IsTransitionAllowed(application.Status, request.TargetStatus))
        {
            return OperationResult.ValidationFailed<ApplicationResponse>(
                $"Cannot move an application from {application.Status} to {request.TargetStatus}.");
        }

        ApplyTransition(application, request);

        _applications.Update(application);
        await _applications.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(application.ToResponse(_transitionPolicy));
    }

    public async Task<OperationResult<ScholarshipStatisticsResponse>> GetStatisticsAsync(
        int scholarshipId,
        CancellationToken cancellationToken)
    {
        var scholarship = await _scholarships.GetByIdAsync(scholarshipId, cancellationToken).ConfigureAwait(false);
        if (scholarship is null)
        {
            return OperationResult.NotFound<ScholarshipStatisticsResponse>(
                $"Scholarship {scholarshipId} was not found.");
        }

        var submitted = await CountAsync(scholarshipId, ApplicationStatus.Submitted, cancellationToken).ConfigureAwait(false);
        var underReview = await CountAsync(scholarshipId, ApplicationStatus.UnderReview, cancellationToken).ConfigureAwait(false);
        var approved = await CountAsync(scholarshipId, ApplicationStatus.Approved, cancellationToken).ConfigureAwait(false);
        var rejected = await CountAsync(scholarshipId, ApplicationStatus.Rejected, cancellationToken).ConfigureAwait(false);

        return OperationResult.Success(new ScholarshipStatisticsResponse
        {
            ScholarshipId = scholarshipId,
            TotalSlots = scholarship.TotalSlots,
            SubmittedCount = submitted,
            UnderReviewCount = underReview,
            ApprovedCount = approved,
            RejectedCount = rejected,
        });
    }

    private Task<int> CountAsync(int scholarshipId, ApplicationStatus status, CancellationToken cancellationToken) =>
        _applications.CountByStatusAsync(scholarshipId, status, cancellationToken);

    /// <summary>Returns a failure result when the caller may not perform the transition, otherwise <see langword="null"/>.</summary>
    private static OperationResult<ApplicationResponse>? AuthoriseTransition(
        CallerContext caller,
        ScholarshipApplication application,
        ApplicationStatus targetStatus)
    {
        if (!caller.CanAccessApplicant(application.ApplicantId))
        {
            return OperationResult.Forbidden<ApplicationResponse>("The application belongs to another applicant.");
        }

        if (!caller.IsAdministrator && AdministratorOnlyTargets.Contains(targetStatus))
        {
            return OperationResult.Forbidden<ApplicationResponse>(
                $"Only an administrator can set status {targetStatus}.");
        }

        return null;
    }

    private void ApplyTransition(ScholarshipApplication application, ApplicationTransitionRequest request)
    {
        application.Status = request.TargetStatus;
        application.UpdatedAtUtc = _clock.UtcNow;

        if (request.TargetStatus == ApplicationStatus.Submitted)
        {
            application.SubmittedAtUtc = _clock.UtcNow;
        }

        if (_transitionPolicy.IsTerminal(request.TargetStatus))
        {
            application.DecidedAtUtc = _clock.UtcNow;
        }

        if (!string.IsNullOrWhiteSpace(request.ReviewerNotes))
        {
            application.ReviewerNotes = request.ReviewerNotes.Trim();
        }
    }
}
