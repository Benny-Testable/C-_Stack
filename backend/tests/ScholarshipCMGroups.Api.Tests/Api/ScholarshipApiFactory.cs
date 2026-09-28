using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Tests.Infrastructure;

namespace ScholarshipCMGroups.Api.Tests.Api;

/// <summary>
/// Hosts the API in-process with an in-memory database.
/// </summary>
/// <remarks>
/// The production <c>Program</c> is used unchanged, so these tests exercise the real middleware
/// order, model binding, authentication, authorization, and serialisation. Only the storage provider
/// is substituted, which is what lets the suite run in CI with no SQL Server instance.
///
/// Settings are supplied through <see cref="IHostBuilder.ConfigureHostConfiguration"/> rather than
/// <c>ConfigureAppConfiguration</c>. The test host turns host configuration into command-line
/// arguments for the entry point, so the values are already present when <c>Program</c> reads
/// <c>builder.Configuration</c> — whereas app configuration is not applied until the host is built,
/// which is after the start-up guards have run.
///
/// Every instance generates its own signing key and its own database name, so two factories are
/// genuinely separate deployments. <see cref="AuthorizationApiTests"/> relies on that to prove a
/// token minted elsewhere is rejected.
/// </remarks>
public sealed class ScholarshipApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"scholarship-api-{Guid.NewGuid():N}";
    private readonly string _signingKey = TestAuthenticationOptions.GenerateSigningKey();
    private readonly int _permitLimit;

    public ScholarshipApiFactory()
        : this(permitLimit: 10_000)
    {
    }

    /// <param name="permitLimit">
    /// Requests allowed per window. The default is high enough that the suite's own volume cannot
    /// trip the limiter; <see cref="RateLimitingApiTests"/> passes a small value on purpose.
    /// </param>
    /// <remarks>
    /// Internal because xUnit requires a class fixture to expose exactly one public constructor.
    /// </remarks>
    internal ScholarshipApiFactory(int permitLimit) => _permitLimit = permitLimit;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureHostConfiguration(configuration => configuration.AddInMemoryCollection(Settings()));

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment(Environments.Development);
        builder.ConfigureServices(services => ReplaceDatabaseWithInMemoryProvider(services, _databaseName));
    }

    /// <summary>Seeds the fixture rows shared by the API tests and returns their identifiers.</summary>
    public async Task<SeedResult> SeedAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ScholarshipDbContext>();

        var open = TestDatabase.OpenScholarship($"Open Programme {Guid.NewGuid():N}");
        var closed = TestDatabase.ClosedScholarship($"Closed Programme {Guid.NewGuid():N}");
        context.Scholarships.AddRange(open, closed);
        await context.SaveChangesAsync();

        return new SeedResult(open.Id, closed.Id);
    }

    /// <summary>Promotes an account to administrator, standing in for out-of-band provisioning.</summary>
    public async Task PromoteToAdministratorAsync(string email)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ScholarshipDbContext>();

        var account = await context.UserAccounts.SingleAsync(u => u.Email == email);
        account.Role = UserRole.Administrator;
        await context.SaveChangesAsync();
    }

    private Dictionary<string, string?> Settings() => new()
    {
        // Replaced below by the in-memory provider; present only so the start-up guard in
        // AddPersistence is satisfied. Contains no credentials.
        ["ConnectionStrings:ScholarshipDatabase"] = "Server=(test);Database=ScholarshipCMGroups",
        ["Database:ApplyMigrationsOnStartup"] = "false",
        ["Authentication:Issuer"] = "scholarship-cmgroups-tests",
        ["Authentication:Audience"] = "scholarship-cmgroups-tests",
        ["Authentication:SigningKey"] = _signingKey,
        ["Authentication:AccessTokenLifetimeMinutes"] = "60",
        ["Authentication:MinimumPasswordLength"] = "12",
        ["RateLimiting:PermitLimit"] = _permitLimit.ToString(System.Globalization.CultureInfo.InvariantCulture),
        ["RateLimiting:WindowSeconds"] = "60",
        ["RateLimiting:QueueLimit"] = "0",
        ["Cors:AllowedOrigins:0"] = "http://localhost:5173",
        // Bootstrap credentials are deliberately absent, matching the shipped configuration.
    };

    private static void ReplaceDatabaseWithInMemoryProvider(IServiceCollection services, string databaseName)
    {
        var registeredOptions = services
            .Where(descriptor => descriptor.ServiceType == typeof(DbContextOptions<ScholarshipDbContext>)
                || descriptor.ServiceType == typeof(DbContextOptions))
            .ToArray();

        foreach (var descriptor in registeredOptions)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<ScholarshipDbContext>(options => options.UseInMemoryDatabase(databaseName));
    }

    public sealed record SeedResult(int OpenScholarshipId, int ClosedScholarshipId);
}
