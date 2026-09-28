using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Repositories;

/// <summary>
/// Queries over the <see cref="Applicant"/> aggregate.
/// </summary>
public interface IApplicantRepository : IRepository<Applicant>
{
    Task<bool> EmailExistsAsync(string email, int? excludingId, CancellationToken cancellationToken);

    /// <summary>Loads an applicant together with their applications, for cascade-delete verification.</summary>
    Task<Applicant?> GetWithApplicationsAsync(int id, CancellationToken cancellationToken);
}

/// <inheritdoc cref="IApplicantRepository"/>
public sealed class ApplicantRepository : EfRepository<Applicant>, IApplicantRepository
{
    public ApplicantRepository(ScholarshipDbContext context)
        : base(context)
    {
    }

    public Task<bool> EmailExistsAsync(string email, int? excludingId, CancellationToken cancellationToken) =>
        Set.AnyAsync(a => a.Email == email && (excludingId == null || a.Id != excludingId), cancellationToken);

    public Task<Applicant?> GetWithApplicationsAsync(int id, CancellationToken cancellationToken) =>
        Set.Include(a => a.Applications).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
}
