using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Repositories;
using ScholarshipCMGroups.Api.Services;
using ScholarshipCMGroups.Api.Tests.Infrastructure;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Services;

/// <summary>
/// Behaviour of the scholarship catalogue service.
/// </summary>
public sealed class ScholarshipServiceTests : IDisposable
{
    private readonly ScholarshipDbContext _context = TestDatabase.CreateContext();
    private readonly TestClock _clock = new();
    private readonly ScholarshipService _service;

    public ScholarshipServiceTests()
    {
        _service = new ScholarshipService(new ScholarshipRepository(_context), _clock);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task CreateAsync_trims_text_and_stamps_the_creation_time()
    {
        var result = await _service.CreateAsync(Request("  Merit Award  ", sponsor: "  CMGroups  "), CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal("Merit Award", result.Value!.Name);
        Assert.Equal("CMGroups", result.Value.SponsorName);
        Assert.Equal(TestClock.DefaultNow, result.Value.CreatedAtUtc);
        Assert.Null(result.Value.UpdatedAtUtc);
    }

    [Fact]
    public async Task CreateAsync_rejects_a_duplicate_name()
    {
        await _service.CreateAsync(Request("Merit Award"), CancellationToken.None);

        var duplicate = await _service.CreateAsync(Request("Merit Award"), CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, duplicate.Status);
        Assert.Equal(1, await _context.Scholarships.CountAsync());
    }

    [Fact]
    public async Task UpdateAsync_stamps_the_update_time_and_keeps_the_creation_time()
    {
        var created = await _service.CreateAsync(Request("Merit Award"), CancellationToken.None);
        _clock.UtcNow = TestClock.DefaultNow.AddDays(1);

        var updated = await _service.UpdateAsync(
            created.Value!.Id,
            Request("Merit Award", sponsor: "New Sponsor"),
            CancellationToken.None);

        Assert.Equal(OperationStatus.Success, updated.Status);
        Assert.Equal("New Sponsor", updated.Value!.SponsorName);
        Assert.Equal(TestClock.DefaultNow, updated.Value.CreatedAtUtc);
        Assert.Equal(TestClock.DefaultNow.AddDays(1), updated.Value.UpdatedAtUtc);
    }

    [Fact]
    public async Task UpdateAsync_allows_a_scholarship_to_keep_its_own_name()
    {
        // The uniqueness check must exclude the row being updated, otherwise no scholarship could
        // ever be edited without also being renamed.
        var created = await _service.CreateAsync(Request("Merit Award"), CancellationToken.None);

        var updated = await _service.UpdateAsync(created.Value!.Id, Request("Merit Award"), CancellationToken.None);

        Assert.Equal(OperationStatus.Success, updated.Status);
    }

    [Fact]
    public async Task UpdateAsync_rejects_a_name_already_used_by_another_scholarship()
    {
        await _service.CreateAsync(Request("First Award"), CancellationToken.None);
        var second = await _service.CreateAsync(Request("Second Award"), CancellationToken.None);

        var result = await _service.UpdateAsync(second.Value!.Id, Request("First Award"), CancellationToken.None);

        Assert.Equal(OperationStatus.Conflict, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_reports_a_missing_scholarship_as_not_found()
    {
        var result = await _service.UpdateAsync(321, Request("Missing Award"), CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetByIdAsync_returns_the_requested_scholarship()
    {
        var created = await _service.CreateAsync(Request("Merit Award"), CancellationToken.None);

        var result = await _service.GetByIdAsync(created.Value!.Id, CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal("Merit Award", result.Value!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_reports_a_missing_scholarship_as_not_found()
    {
        var result = await _service.GetByIdAsync(999, CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task DeleteAsync_removes_the_scholarship()
    {
        var created = await _service.CreateAsync(Request("Merit Award"), CancellationToken.None);

        var result = await _service.DeleteAsync(created.Value!.Id, CancellationToken.None);

        Assert.Equal(OperationStatus.Success, result.Status);
        Assert.Equal(0, await _context.Scholarships.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_reports_a_missing_scholarship_as_not_found()
    {
        var result = await _service.DeleteAsync(456, CancellationToken.None);

        Assert.Equal(OperationStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task GetPageAsync_filters_by_active_state()
    {
        await _service.CreateAsync(Request("Active Award"), CancellationToken.None);
        await _service.CreateAsync(Request("Retired Award", isActive: false), CancellationToken.None);

        var activeOnly = await _service.GetPageAsync(null, null, true, null, CancellationToken.None);
        var everything = await _service.GetPageAsync(null, null, null, null, CancellationToken.None);

        Assert.Equal(1, activeOnly.TotalCount);
        Assert.Equal("Active Award", Assert.Single(activeOnly.Items).Name);
        Assert.Equal(2, everything.TotalCount);
    }

    [Fact]
    public async Task GetPageAsync_filters_by_search_term()
    {
        await _service.CreateAsync(Request("Engineering Access Grant"), CancellationToken.None);
        await _service.CreateAsync(Request("Community Service Award"), CancellationToken.None);

        // Matched with the stored casing on purpose: case sensitivity of Contains is decided by the
        // database collation, so asserting a lowercase match here would be asserting a provider
        // detail that the in-memory provider and SQL Server disagree about.
        var matches = await _service.GetPageAsync(null, null, null, "Engineering", CancellationToken.None);

        Assert.Equal(1, matches.TotalCount);
        Assert.Equal("Engineering Access Grant", Assert.Single(matches.Items).Name);
    }

    [Fact]
    public async Task GetPageAsync_reports_paging_metadata_across_multiple_pages()
    {
        for (var index = 0; index < 5; index++)
        {
            await _service.CreateAsync(Request(FormattableString.Invariant($"Award {index}")), CancellationToken.None);
        }

        var firstPage = await _service.GetPageAsync(1, 2, null, null, CancellationToken.None);
        var lastPage = await _service.GetPageAsync(3, 2, null, null, CancellationToken.None);

        Assert.Equal(5, firstPage.TotalCount);
        Assert.Equal(3, firstPage.TotalPages);
        Assert.True(firstPage.HasNextPage);
        Assert.Equal(2, firstPage.Items.Count);

        Assert.False(lastPage.HasNextPage);
        Assert.Single(lastPage.Items);
    }

    [Fact]
    public async Task CreateAsync_rejects_a_null_request()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _service.CreateAsync(null!, CancellationToken.None));
    }

    private static ScholarshipRequest Request(
        string name,
        string sponsor = "CMGroups Foundation",
        bool isActive = true) => new()
    {
        Name = name,
        SponsorName = sponsor,
        Description = "A scholarship used by the automated test suite.",
        AwardAmount = 5000m,
        TotalSlots = 10,
        ApplicationOpensOn = new DateOnly(2026, 1, 1),
        ApplicationClosesOn = new DateOnly(2026, 12, 31),
        IsActive = isActive,
    };
}
