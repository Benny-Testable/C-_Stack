using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Repositories;

/// <summary>
/// Queries over the <see cref="UserAccount"/> aggregate.
/// </summary>
public interface IUserAccountRepository : IRepository<UserAccount>
{
    Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> AnyAdministratorExistsAsync(CancellationToken cancellationToken);
}

/// <inheritdoc cref="IUserAccountRepository"/>
public sealed class UserAccountRepository : EfRepository<UserAccount>, IUserAccountRepository
{
    public UserAccountRepository(ScholarshipDbContext context)
        : base(context)
    {
    }

    public Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
        Set.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public Task<bool> AnyAdministratorExistsAsync(CancellationToken cancellationToken) =>
        Set.AnyAsync(u => u.Role == UserRole.Administrator, cancellationToken);
}
