using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Configuration;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Startup;

/// <summary>
/// Applies pending migrations and provisions the first administrator.
/// </summary>
public static class DatabaseInitializer
{
    /// <summary>
    /// Runs migrations when <c>Database:ApplyMigrationsOnStartup</c> is enabled, then creates the
    /// bootstrap administrator if one is configured and none exists yet.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(services);

        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;

        var configuration = provider.GetRequiredService<IConfiguration>();
        if (configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
        {
            var context = provider.GetRequiredService<ScholarshipDbContext>();
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
        }

        await EnsureAdministratorAsync(provider, cancellationToken).ConfigureAwait(false);
    }

    private static async Task EnsureAdministratorAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        var bootstrap = provider.GetRequiredService<IOptions<BootstrapOptions>>().Value;
        var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(DatabaseInitializer));

        if (!bootstrap.IsConfigured)
        {
            LogMessages.BootstrapAdministratorSkipped(logger);
            return;
        }

        var context = provider.GetRequiredService<ScholarshipDbContext>();
        if (await context.UserAccounts.AnyAsync(u => u.Role == UserRole.Administrator, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var hasher = provider.GetRequiredService<IPasswordHasher<UserAccount>>();
        var clock = provider.GetRequiredService<IClock>();

        var administrator = new UserAccount
        {
            Email = bootstrap.AdministratorEmail!.Trim().ToLowerInvariant(),
            Role = UserRole.Administrator,
            CreatedAtUtc = clock.UtcNow,
        };
        administrator.PasswordHash = hasher.HashPassword(administrator, bootstrap.AdministratorPassword!);

        context.UserAccounts.Add(administrator);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Logs the account id rather than the email address, keeping PII out of the log stream.
        LogMessages.BootstrapAdministratorCreated(logger, administrator.Id);
    }
}
