using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Tests.Infrastructure;

/// <summary>
/// Creates isolated EF Core contexts and the fixture rows the service tests operate on.
/// </summary>
/// <remarks>
/// Each context gets its own database name so tests never observe each other's writes, which keeps
/// the suite safe to run in parallel.
///
/// The in-memory provider is used because the relational behaviour the service layer depends on
/// (change tracking, cascade delete of tracked graphs, LINQ translation) is provider-independent.
/// Behaviour that genuinely is SQL Server specific — unique index enforcement, rowversion
/// concurrency tokens — is asserted against the EF model metadata in
/// <see cref="Data.ScholarshipDbContextTests"/> and against the generated script in
/// database/schema, rather than pretended here.
/// </remarks>
internal static class TestDatabase
{
    public static ScholarshipDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ScholarshipDbContext>()
            .UseInMemoryDatabase($"scholarship-tests-{Guid.NewGuid():N}")
            .EnableSensitiveDataLogging(false)
            .Options;

        return new ScholarshipDbContext(options);
    }

    /// <summary>A scholarship whose application window is open on <see cref="TestClock.DefaultNow"/>.</summary>
    public static Scholarship OpenScholarship(string name = "Open Programme") => new()
    {
        Name = name,
        SponsorName = "CMGroups Foundation",
        AwardAmount = 5000m,
        TotalSlots = 10,
        ApplicationOpensOn = new DateOnly(2026, 1, 1),
        ApplicationClosesOn = new DateOnly(2026, 12, 31),
        IsActive = true,
        CreatedAtUtc = TestClock.DefaultNow,
    };

    /// <summary>A scholarship whose window closed before <see cref="TestClock.DefaultNow"/>.</summary>
    public static Scholarship ClosedScholarship(string name = "Closed Programme") => new()
    {
        Name = name,
        SponsorName = "CMGroups Foundation",
        AwardAmount = 2000m,
        TotalSlots = 5,
        ApplicationOpensOn = new DateOnly(2025, 1, 1),
        ApplicationClosesOn = new DateOnly(2025, 6, 30),
        IsActive = true,
        CreatedAtUtc = TestClock.DefaultNow,
    };

    public static Applicant Applicant(string email) => new()
    {
        FullName = "Test Applicant",
        Email = email,
        InstitutionName = "Test Institute",
        CreatedAtUtc = TestClock.DefaultNow,
    };
}
