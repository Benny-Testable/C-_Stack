using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Configuration;
using ScholarshipCMGroups.Api.Data;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Repositories;
using ScholarshipCMGroups.Api.Services;

namespace ScholarshipCMGroups.Api.Startup;

/// <summary>
/// Registration of every service the API depends on, grouped by concern.
/// </summary>
public static class ServiceCollectionExtensions
{
    internal const string CorsPolicyName = "ScholarshipCmGroupsFrontend";

    /// <summary>Binds and eagerly validates the strongly typed configuration sections.</summary>
    public static IServiceCollection AddApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // ValidateOnStart turns a missing signing key into a start-up failure instead of a runtime
        // 500, and removes any temptation to ship a fallback key in source control.
        services.AddOptions<AuthenticationOptions>()
            .Bind(configuration.GetSection(AuthenticationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<RateLimitingOptions>()
            .Bind(configuration.GetSection(RateLimitingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<CorsOptions>(configuration.GetSection(CorsOptions.SectionName));
        services.Configure<BootstrapOptions>(configuration.GetSection(BootstrapOptions.SectionName));

        return services;
    }

    /// <summary>Registers the EF Core context against SQL Server.</summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("ScholarshipDatabase");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ScholarshipDatabase' is not configured. See docs/setup.md.");
        }

        services.AddDbContext<ScholarshipDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        return services;
    }

    /// <summary>Registers repositories, domain services, and their collaborators.</summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IApplicationStatusTransitionPolicy, ApplicationStatusTransitionPolicy>();
        services.AddSingleton<IPasswordHasher<UserAccount>, PasswordHasher<UserAccount>>();

        services.AddScoped<IScholarshipRepository, ScholarshipRepository>();
        services.AddScoped<IApplicantRepository, ApplicantRepository>();
        services.AddScoped<IScholarshipApplicationRepository, ScholarshipApplicationRepository>();
        services.AddScoped<IUserAccountRepository, UserAccountRepository>();

        services.AddScoped<IScholarshipService, ScholarshipService>();
        services.AddScoped<IApplicantService, ApplicantService>();
        services.AddScoped<IScholarshipApplicationService, ScholarshipApplicationService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenService, TokenService>();

        return services;
    }

    /// <summary>Configures JWT bearer authentication and role-based authorization.</summary>
    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = configuration.GetSection(AuthenticationOptions.SectionName).Get<AuthenticationOptions>()
            ?? throw new InvalidOperationException(
                $"Configuration section '{AuthenticationOptions.SectionName}' is missing. See docs/setup.md.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.MapInboundClaims = false;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = options.Issuer,
                    ValidateAudience = true,
                    ValidAudience = options.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey)),
                    ValidateLifetime = true,
                    // No leeway: an expired token must be rejected immediately, which is what the
                    // Excel "Session Timeout Compliance Rate" metric asserts.
                    ClockSkew = TimeSpan.Zero,
                };
            });

        services.AddAuthorization();

        return services;
    }

    /// <summary>Applies a global fixed-window request limit.</summary>
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var limits = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>()
            ?? new RateLimitingOptions();

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    PartitionKey(context),
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = limits.PermitLimit,
                        Window = TimeSpan.FromSeconds(limits.WindowSeconds),
                        QueueLimit = limits.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    }));
        });

        return services;
    }

    /// <summary>Registers an explicit origin allow-list for the React client.</summary>
    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var origins = configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>()?.AllowedOrigins
            ?? Array.Empty<string>();

        services.AddCors(cors => cors.AddPolicy(CorsPolicyName, policy =>
        {
            if (origins.Length == 0)
            {
                // No origins configured means no cross-origin access, rather than a wildcard.
                policy.WithOrigins(Array.Empty<string>());
                return;
            }

            policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
        }));

        return services;
    }

    /// <summary>Adds the OpenAPI document that backs the contract-conformance metric.</summary>
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Scholarship CMGroups API",
                Version = "v1",
                Description = "Scholarship programme, applicant, and application management.",
            });

            var scheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste the access token returned by POST /api/auth/login.",
                Reference = new OpenApiReference { Id = JwtBearerDefaults.AuthenticationScheme, Type = ReferenceType.SecurityScheme },
            };

            swagger.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, scheme);
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement { [scheme] = Array.Empty<string>() });
        });

        return services;
    }

    /// <summary>Partitions the rate limiter by authenticated subject, falling back to remote address.</summary>
    private static string PartitionKey(HttpContext context) =>
        context.User.Identity?.IsAuthenticated == true
            ? context.User.FindFirst("sub")?.Value ?? "authenticated"
            : context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
}
