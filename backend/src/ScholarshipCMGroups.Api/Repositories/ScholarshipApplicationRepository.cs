using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Repositories;

/// <summary>
/// Queries over the <see cref="ScholarshipApplication"/> aggregate.
/// </summary>
public interface IScholarshipApplicationRepository : IRepository<ScholarshipApplication>
{
    Task<(IReadOnlyList<ScholarshipApplication> Items, int TotalCount)> GetPageAsync(
        PageRequest page,
        int? applicantId,
        int? scholarshipId,
        ApplicationStatus? status,
        CancellationToken cancellationToken);

    /// <summary>Loads an application with its scholarship eagerly, avoiding a follow-up query per row.</summary>
    Task<ScholarshipApplication?> GetWithScholarshipAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExistsForApplicantAsync(int scholarshipId, int applicantId, CancellationToken cancellationToken);

    Task<int> CountByStatusAsync(int scholarshipId, ApplicationStatus status, CancellationToken cancellationToken);
}

/// <inheritdoc cref="IScholarshipApplicationRepository"/>
public sealed class ScholarshipApplicationRepository
    : EfRepository<ScholarshipApplication>, IScholarshipApplicationRepository
{
    public ScholarshipApplicationRepository(ScholarshipDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<ScholarshipApplication> Items, int TotalCount)> GetPageAsync(
        PageRequest page,
        int? applicantId,
        int? scholarshipId,
        ApplicationStatus? status,
        CancellationToken cancellationToken)
    {
        var query = Set.AsQueryable();

        if (applicantId is not null)
        {
            query = query.Where(a => a.ApplicantId == applicantId.Value);
        }

        if (scholarshipId is not null)
        {
            query = query.Where(a => a.ScholarshipId == scholarshipId.Value);
        }

        if (status is not null)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await query
            .Include(a => a.Scholarship)
            .OrderByDescending(a => a.CreatedAtUtc)
            .ThenBy(a => a.Id)
            .Skip(page.Skip)
            .Take(page.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public Task<ScholarshipApplication?> GetWithScholarshipAsync(int id, CancellationToken cancellationToken) =>
        Set.Include(a => a.Scholarship).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<bool> ExistsForApplicantAsync(int scholarshipId, int applicantId, CancellationToken cancellationToken) =>
        Set.AnyAsync(a => a.ScholarshipId == scholarshipId && a.ApplicantId == applicantId, cancellationToken);

    public Task<int> CountByStatusAsync(int scholarshipId, ApplicationStatus status, CancellationToken cancellationToken) =>
        Set.CountAsync(a => a.ScholarshipId == scholarshipId && a.Status == status, cancellationToken);
}
