using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WestCoastFitness.Application;
using WestCoastFitness.Domain;

namespace WestCoastFitness.Infrastructure;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

public sealed class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<Member> _inner = new();

    public string Hash(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        return _inner.HashPassword(new Member(), password);
    }

    public bool Verify(string passwordHash, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(passwordHash);
        ArgumentException.ThrowIfNullOrEmpty(password);
        var result = _inner.VerifyHashedPassword(new Member(), passwordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}

public sealed class MockPaymentGateway : IPaymentGateway
{
    public Task<PaymentCapture> CaptureAsync(decimal amount, string reference, CancellationToken cancellationToken)
    {
        if (amount <= 0)
        {
            return Task.FromResult(new PaymentCapture(false, string.Empty, "The payment amount must be greater than zero."));
        }

        var providerReference = "mock-" + Guid.NewGuid().ToString("N");
        return Task.FromResult(new PaymentCapture(true, providerReference, string.Empty));
    }
}

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        var connection = configuration.GetConnectionString("Postgres");
        if (string.IsNullOrWhiteSpace(connection))
        {
            throw new InvalidOperationException("ConnectionStrings:Postgres is required.");
        }

        services.AddDbContext<FitnessDbContext>(options => options.UseNpgsql(connection));
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IMembershipRepository, MembershipRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<IAssessmentRepository, AssessmentRepository>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();
        services.AddSingleton<IPaymentGateway, MockPaymentGateway>();
        return services;
    }
}
