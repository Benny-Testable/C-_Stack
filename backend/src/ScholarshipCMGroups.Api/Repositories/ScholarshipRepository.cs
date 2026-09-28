using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Repositories;

/// <summary>
/// Queries over the <see cref="Scholarship"/> aggregate.
/// </summary>
public interface IScholarshipRepository : IRepository<Scholarship>
{
    Task<(IReadOnlyList<Scholarship> Items, int TotalCount)> GetPageAsync(
        PageRequest page,
        bool? activeOnly,
        string? searchTerm,
        CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, int? excludingId, CancellationToken cancellationToken);
}

/// <inheritdoc cref="IScholarshipRepository"/>
public sealed class ScholarshipRepository : EfRepository<Scholarship>, IScholarshipRepository
{
    public ScholarshipRepository(ScholarshipDbContext context)
        : base(context)
    {
    }

    public async Task<(IReadOnlyList<Scholarship> Items, int TotalCount)> GetPageAsync(
        PageRequest page,
        bool? activeOnly,
        string? searchTerm,
        CancellationToken cancellationToken)
    {
        var query = BuildQuery(activeOnly, searchTerm);

        // Counting and paging are two round-trips against the same filtered query rather than a
        // fetch-then-filter in memory, which is what the Excel N+1 / query-analysis metric looks for.
        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        var items = await query
            .OrderBy(s => s.ApplicationClosesOn)
            .ThenBy(s => s.Id)
            .Skip(page.Skip)
            .Take(page.PageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return (items, totalCount);
    }

    public Task<bool> NameExistsAsync(string name, int? excludingId, CancellationToken cancellationToken) =>
        Set.AnyAsync(s => s.Name == name && (excludingId == null || s.Id != excludingId), cancellationToken);

    private IQueryable<Scholarship> BuildQuery(bool? activeOnly, string? searchTerm)
    {
        var query = Set.AsQueryable();

        if (activeOnly is not null)
        {
            query = query.Where(s => s.IsActive == activeOnly.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Parameterised by EF Core; the term is never concatenated into SQL.
            var term = searchTerm.Trim();
            query = query.Where(s => s.Name.Contains(term) || s.SponsorName.Contains(term));
        }

        return query;
    }
}
