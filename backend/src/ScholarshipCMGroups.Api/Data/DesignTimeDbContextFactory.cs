using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ScholarshipCMGroups.Api.Data;

/// <summary>
/// Supplies a context to <c>dotnet ef</c> at design time.
/// </summary>
/// <remarks>
/// Migrations are generated from the model, not from a live server, so the placeholder below is
/// only used to select the SQL Server provider's SQL dialect. Having this factory means the design
/// time tooling never needs a real connection string or signing key, which keeps credentials out of
/// the repository entirely.
/// </remarks>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ScholarshipDbContext>
{
    private const string DesignTimePlaceholderConnection =
        "Server=(design-time);Database=ScholarshipCMGroups;Trusted_Connection=True;TrustServerCertificate=True";

    public ScholarshipDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ScholarshipDbContext>()
            .UseSqlServer(
                Environment.GetEnvironmentVariable("SCHOLARSHIP_DB_CONNECTION") ?? DesignTimePlaceholderConnection)
            .Options;

        return new ScholarshipDbContext(options);
    }
}
