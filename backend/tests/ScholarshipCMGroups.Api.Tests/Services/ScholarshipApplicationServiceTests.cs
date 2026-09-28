using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Repositories;
using ScholarshipCMGroups.Api.Services;
using ScholarshipCMGroups.Api.Tests.Infrastructure;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Services;

/// <summary>
/// Behaviour of the application lifecycle service, including ownership enforcement.
/// </summary>
public sealed class ScholarshipApplicationServiceTests : IDisposable
{
    private readonly ScholarshipDbContext _context = TestDatabase.CreateContext();
    private readonly TestClock _clock = new();
    private readonly ScholarshipApplicationService _service;

    public ScholarshipApplicationServiceTests()
    {
        _service = new ScholarshipApplicationService(
            new ScholarshipApplicationRepository(_context),
            new ScholarshipRepository(_context),
            new ApplicantRepository(_context),
            new ApplicationStatusTransitionPolicy(),
            _clock);
    }

    public void Dispose() => _context.Dispose();

    // -----------------------------------------------------------------------
    // Creating drafts
    // -----------------------------------------------------------------------

    [Fact]
    public async Task CreateDraftAsync_opens_a_draft_when_the_window_is_open()
    {
        var (scholarshipId, applicantId) = await SeedAsync();

        var result = await _service.CreateDraftAsync(
            Applicant(applicantId),
            new ApplicationCreateRequest
            {
                ScholarshipId = scholarshipId,
                ApplicantId = applicantId,
                Motivation = "  I would use this award to finish my final year project.  ",
            },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal(ApplicationStatus.Draft, result.Value!.Status);
        Assert.Equal(
            "I would use this award to finish my final year project.",
            result.Value.Motivation);
        Assert.Null(result.Value.SubmittedAtUtc);
        Assert.Equal(TestClock.DefaultNow, result.Value.CreatedAtUtc);
    }

    [Fact]
    public async Task CreateDraftAsync_rejects_a_scholarship_whose_window_has_closed()
    {
        var scholarship = TestDatabase.ClosedScholarship();
        var applicant = TestDatabase.Applicant("closed-window@example.test");
        _context.Scholarships.Add(scholarship);
        _context.Applicants.Add(applicant);
        await _context.SaveChangesAsync();

        var result = await _service.CreateDraftAsync(
            Applicant(applicant.Id),
            new ApplicationCreateRequest { ScholarshipId = scholarship.Id, ApplicantId = applicant.Id },
            CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
        Assert.Equal(0, await _context.Applications.CountAsync());
    }

    [Fact]
    public async Task CreateDraftAsync_rejects_a_second_application_for_the_same_scholarship()
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var request = new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = applicantId };

        var first = await _service.CreateDraftAsync(Applicant(applicantId), request, CancellationToken.None);
        var second = await _service.CreateDraftAsync(Applicant(applicantId), request, CancellationToken.None);

        Assert.Equal(OperationStatus.Success, first.Status);
        Assert.Equal(OperationStatus.Conflict, second.Status);
    }

    [Fact]
    public async Task CreateDraftAsync_reports_a_missing_scholarship_as_not_found()
    {
        var (_, applicantId) = await SeedAsync();

        var result = await _service.CreateDraftAsync(
            Applicant(applicantId),
            new ApplicationCreateRequest { ScholarshipId = 4242, ApplicantId = applicantId },
            CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task CreateDraftAsync_refuses_to_create_an_application_for_another_applicant()
    {
        // Broken-object-level-authorisation guard: the body names a different applicant than the
        // token. Expected value for the Excel "BOLA Finding Count" metric is zero, so this must be
        // refused before any row is written.
        var (scholarshipId, applicantId) = await SeedAsync();
        var otherApplicant = TestDatabase.Applicant("other-owner@example.test");
        _context.Applicants.Add(otherApplicant);
        await _context.SaveChangesAsync();

        var result = await _service.CreateDraftAsync(
            Applicant(applicantId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = otherApplicant.Id },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Forbidden, result.Status);
        Assert.Equal(0, await _context.Applications.CountAsync());
    }

    // -----------------------------------------------------------------------
    // Reading
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetByIdAsync_refuses_to_return_another_applicants_application()
    {
        var (scholarshipId, ownerId) = await SeedAsync();
        var intruder = TestDatabase.Applicant("intruder@example.test");
        _context.Applicants.Add(intruder);
        await _context.SaveChangesAsync();

        var created = await _service.CreateDraftAsync(
            Applicant(ownerId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = ownerId },
            CancellationToken.None);

        var result = await _service.GetByIdAsync(Applicant(intruder.Id), created.Value!.Id, CancellationToken.None);

        Assert.Equal(OperationStatus.Forbidden, result.Status);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_allows_an_administrator_to_read_any_application()
    {
        var (scholarshipId, ownerId) = await SeedAsync();
        var created = await _service.CreateDraftAsync(
            Applicant(ownerId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = ownerId },
            CancellationToken.None);

        var result = await _service.GetByIdAsync(Administrator(), created.Value!.Id, CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal(created.Value.Id, result.Value!.Id);
    }

    [Fact]
    public async Task GetPageAsync_shows_an_applicant_only_their_own_applications()
    {
        var (scholarshipId, ownerId) = await SeedAsync();
        var second = TestDatabase.Applicant("second-owner@example.test");
        var secondScholarship = TestDatabase.OpenScholarship("Second Programme");
        _context.Applicants.Add(second);
        _context.Scholarships.Add(secondScholarship);
        await _context.SaveChangesAsync();

        await _service.CreateDraftAsync(
            Applicant(ownerId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = ownerId },
            CancellationToken.None);
        await _service.CreateDraftAsync(
            Applicant(second.Id),
            new ApplicationCreateRequest { ScholarshipId = secondScholarship.Id, ApplicantId = second.Id },
            CancellationToken.None);

        var ownerPage = await _service.GetPageAsync(Applicant(ownerId), null, null, null, null, CancellationToken.None);
        var adminPage = await _service.GetPageAsync(Administrator(), null, null, null, null, CancellationToken.None);

        Assert.Equal(1, ownerPage.TotalCount);
        Assert.All(ownerPage.Items, item => Assert.Equal(ownerId, item.ApplicantId));
        Assert.Equal(2, adminPage.TotalCount);
    }

    [Fact]
    public async Task GetPageAsync_clamps_an_oversized_page_size()
    {
        await SeedAsync();

        var page = await _service.GetPageAsync(Administrator(), 0, 5000, null, null, CancellationToken.None);

        Assert.Equal(1, page.Page);
        Assert.Equal(PageRequest.MaxPageSize, page.PageSize);
    }

    // -----------------------------------------------------------------------
    // Editing drafts
    // -----------------------------------------------------------------------

    [Fact]
    public async Task UpdateDraftAsync_rejects_an_edit_once_the_application_left_draft()
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var created = await _service.CreateDraftAsync(
            Applicant(applicantId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = applicantId },
            CancellationToken.None);

        await _service.TransitionAsync(
            Applicant(applicantId),
            created.Value!.Id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            CancellationToken.None);

        var result = await _service.UpdateDraftAsync(
            Applicant(applicantId),
            created.Value.Id,
            new ApplicationUpdateRequest { Motivation = "A late change." },
            CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
    }

    // -----------------------------------------------------------------------
    // Transitions
    // -----------------------------------------------------------------------

    [Fact]
    public async Task TransitionAsync_stamps_the_submission_time_on_submit()
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var created = await _service.CreateDraftAsync(
            Applicant(applicantId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = applicantId },
            CancellationToken.None);

        var result = await _service.TransitionAsync(
            Applicant(applicantId),
            created.Value!.Id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal(ApplicationStatus.Submitted, result.Value!.Status);
        Assert.Equal(TestClock.DefaultNow, result.Value.SubmittedAtUtc);
        Assert.Null(result.Value.DecidedAtUtc);
    }

    [Fact]
    public async Task TransitionAsync_stamps_the_decision_time_on_a_terminal_status()
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var id = await CreateSubmittedAsync(scholarshipId, applicantId);

        await _service.TransitionAsync(
            Administrator(),
            id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.UnderReview },
            CancellationToken.None);

        _clock.UtcNow = TestClock.DefaultNow.AddHours(3);

        var result = await _service.TransitionAsync(
            Administrator(),
            id,
            new ApplicationTransitionRequest
            {
                TargetStatus = ApplicationStatus.Approved,
                ReviewerNotes = "  Strong academic record.  ",
            },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal(TestClock.DefaultNow.AddHours(3), result.Value!.DecidedAtUtc);
        Assert.Equal("Strong academic record.", result.Value.ReviewerNotes);
        Assert.Empty(result.Value.AllowedNextStatuses);
    }

    [Fact]
    public async Task TransitionAsync_rejects_a_transition_the_state_machine_does_not_permit()
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var created = await _service.CreateDraftAsync(
            Applicant(applicantId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = applicantId },
            CancellationToken.None);

        // Draft -> Approved skips submission and review, so it must be refused.
        var result = await _service.TransitionAsync(
            Administrator(),
            created.Value!.Id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Approved },
            CancellationToken.None);

        Assert.Equal(OperationStatus.ValidationFailed, result.Status);
    }

    [Theory]
    [InlineData(ApplicationStatus.UnderReview)]
    [InlineData(ApplicationStatus.Approved)]
    [InlineData(ApplicationStatus.Rejected)]
    public async Task TransitionAsync_reserves_review_outcomes_for_administrators(ApplicationStatus target)
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var id = await CreateSubmittedAsync(scholarshipId, applicantId);

        var result = await _service.TransitionAsync(
            Applicant(applicantId),
            id,
            new ApplicationTransitionRequest { TargetStatus = target },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task TransitionAsync_lets_an_applicant_withdraw_their_own_application()
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var id = await CreateSubmittedAsync(scholarshipId, applicantId);

        var result = await _service.TransitionAsync(
            Applicant(applicantId),
            id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Withdrawn },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal(ApplicationStatus.Withdrawn, result.Value!.Status);
    }

    [Fact]
    public async Task TransitionAsync_refuses_to_act_on_another_applicants_application()
    {
        var (scholarshipId, ownerId) = await SeedAsync();
        var intruder = TestDatabase.Applicant("transition-intruder@example.test");
        _context.Applicants.Add(intruder);
        await _context.SaveChangesAsync();

        var id = await CreateSubmittedAsync(scholarshipId, ownerId);

        var result = await _service.TransitionAsync(
            Applicant(intruder.Id),
            id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Withdrawn },
            CancellationToken.None);

        Assert.Equal(OperationStatus.Forbidden, result.Status);
    }

    [Fact]
    public async Task TransitionAsync_reports_a_missing_application_as_not_found()
    {
        var result = await _service.TransitionAsync(
            Administrator(),
            5150,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    // -----------------------------------------------------------------------
    // Statistics
    // -----------------------------------------------------------------------

    [Fact]
    public async Task GetStatisticsAsync_counts_applications_by_status()
    {
        var (scholarshipId, applicantId) = await SeedAsync();
        var id = await CreateSubmittedAsync(scholarshipId, applicantId);

        var statistics = await _service.GetStatisticsAsync(scholarshipId, CancellationToken.None);

        Assert.Equal(OperationStatus.Success, statistics.Status);
        Assert.Equal(1, statistics.Value!.SubmittedCount);
        Assert.Equal(0, statistics.Value.ApprovedCount);
        Assert.Equal(10, statistics.Value.TotalSlots);
        Assert.Equal(10, statistics.Value.RemainingSlots);

        await _service.TransitionAsync(
            Administrator(),
            id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.UnderReview },
            CancellationToken.None);
        await _service.TransitionAsync(
            Administrator(),
            id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Approved },
            CancellationToken.None);

        var afterApproval = await _service.GetStatisticsAsync(scholarshipId, CancellationToken.None);

        Assert.Equal(1, afterApproval.Value!.ApprovedCount);
        Assert.Equal(9, afterApproval.Value.RemainingSlots);
    }

    [Fact]
    public async Task GetStatisticsAsync_reports_a_missing_scholarship_as_not_found()
    {
        var statistics = await _service.GetStatisticsAsync(777, CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, statistics.Status);
    }

    // -----------------------------------------------------------------------

    private static CallerContext Applicant(int applicantId) => new(UserRole.Applicant, applicantId);

    private static CallerContext Administrator() => new(UserRole.Administrator, null);

    private async Task<(int ScholarshipId, int ApplicantId)> SeedAsync()
    {
        var scholarship = TestDatabase.OpenScholarship();
        var applicant = TestDatabase.Applicant("owner@example.test");

        _context.Scholarships.Add(scholarship);
        _context.Applicants.Add(applicant);
        await _context.SaveChangesAsync();

        return (scholarship.Id, applicant.Id);
    }

    private async Task<int> CreateSubmittedAsync(int scholarshipId, int applicantId)
    {
        var created = await _service.CreateDraftAsync(
            Applicant(applicantId),
            new ApplicationCreateRequest { ScholarshipId = scholarshipId, ApplicantId = applicantId },
            CancellationToken.None);

        await _service.TransitionAsync(
            Applicant(applicantId),
            created.Value!.Id,
            new ApplicationTransitionRequest { TargetStatus = ApplicationStatus.Submitted },
            CancellationToken.None);

        return created.Value.Id;
    }
}
