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
/// Behaviour of the applicant service, including the erasure path.
/// </summary>
public sealed class ApplicantServiceTests : IDisposable
{
    private readonly ScholarshipDbContext _context = TestDatabase.CreateContext();
    private readonly TestClock _clock = new();
    private readonly ApplicantService _service;

    public ApplicantServiceTests()
    {
        _service = new ApplicantService(new ApplicantRepository(_context), _clock);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task CreateAsync_normalises_the_email_to_lower_case()
    {
        // Emails are stored normalised so that the unique index behaves as a true identity
        // constraint rather than allowing Ada@x and ada@x to coexist.
        var result = await _service.CreateAsync(
            Request("  Ada Lovelace  ", "  Ada.Lovelace@Example.TEST  "),
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal("Ada Lovelace", result.Value!.FullName);
        Assert.Equal("ada.lovelace@example.test", result.Value.Email);
    }

    [Fact]
    public async Task CreateAsync_rejects_an_email_that_differs_only_by_case()
    {
        await _service.CreateAsync(Request("First", "person@example.test"), CancellationToken.None);

        var duplicate = await _service.CreateAsync(Request("Second", "PERSON@example.test"), CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, duplicate.Status);
        Assert.Equal(1, await _context.Applicants.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_allows_an_applicant_to_keep_their_own_email()
    {
        var created = await _service.CreateAsync(Request("Ada", "ada@example.test"), CancellationToken.None);

        var updated = await _service.UpdateAsync(
            created.Value!.Id,
            Request("Ada Byron", "ada@example.test"),
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, updated.Status);
        Assert.Equal("Ada Byron", updated.Value!.FullName);
    }

    [Fact]
    public async Task UpdateAsync_rejects_an_email_belonging_to_another_applicant()
    {
        await _service.CreateAsync(Request("First", "first@example.test"), CancellationToken.None);
        var second = await _service.CreateAsync(Request("Second", "second@example.test"), CancellationToken.None);

        var result = await _service.UpdateAsync(
            second.Value!.Id,
            Request("Second", "first@example.test"),
            CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_reports_a_missing_applicant_as_not_found()
    {
        var result = await _service.UpdateAsync(88, Request("Nobody", "nobody@example.test"), CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_reports_a_missing_applicant_as_not_found()
    {
        var result = await _service.GetByIdAsync(99, CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task EraseAsync_removes_the_applicant_and_every_dependent_row()
    {
        // Evidence for the Excel "Data Deletion Verification Rate" metric, whose expected value is
        // 100% of the subject's data removed. The assertion therefore checks all three tables that
        // can hold a reference to the applicant, not just the applicant row itself.
        var applicant = TestDatabase.Applicant("erase-me@example.test");
        var scholarship = TestDatabase.OpenScholarship();
        _context.Applicants.Add(applicant);
        _context.Scholarships.Add(scholarship);
        await _context.SaveChangesAsync();

        _context.Applications.Add(new ScholarshipApplication
        {
            ApplicantId = applicant.Id,
            ScholarshipId = scholarship.Id,
            Status = ApplicationStatus.Draft,
            CreatedAtUtc = TestClock.DefaultNow,
        });
        _context.UserAccounts.Add(new UserAccount
        {
            Email = applicant.Email,
            PasswordHash = "not-a-real-hash",
            Role = UserRole.Applicant,
            ApplicantId = applicant.Id,
            CreatedAtUtc = TestClock.DefaultNow,
        });
        await _context.SaveChangesAsync();

        var receipt = await _service.EraseAsync(applicant.Id, CancellationToken.None);

        Assert.Equal(OperationStatus.Success, receipt.Status);
        Assert.Equal(applicant.Id, receipt.Value!.ApplicantId);
        Assert.Equal(1, receipt.Value.ApplicationsDeleted);
        Assert.Equal(TestClock.DefaultNow, receipt.Value.ErasedAtUtc);

        Assert.Equal(0, await _context.Applicants.CountAsync());
        Assert.Equal(0, await _context.Applications.CountAsync());
        Assert.Equal(0, await _context.UserAccounts.CountAsync());

        // The scholarship is reference data, not personal data, so erasure must leave it intact.
        Assert.Equal(1, await _context.Scholarships.CountAsync());
    }

    [Fact]
    public async Task EraseAsync_reports_zero_deletions_for_an_applicant_with_no_applications()
    {
        var created = await _service.CreateAsync(Request("Solo", "solo@example.test"), CancellationToken.None);

        var receipt = await _service.EraseAsync(created.Value!.Id, CancellationToken.None);

        Assert.Equal(OperationStatus.Success, receipt.Status);
        Assert.Equal(0, receipt.Value!.ApplicationsDeleted);
    }

    [Fact]
    public async Task EraseAsync_reports_a_missing_applicant_as_not_found()
    {
        var receipt = await _service.EraseAsync(1234, CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, receipt.Status);
    }

    private static ApplicantRequest Request(string fullName, string email) => new()
    {
        FullName = fullName,
        Email = email,
        InstitutionName = "Test Institute",
    };
}
