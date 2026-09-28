using System.ComponentModel.DataAnnotations;

namespace ScholarshipCMGroups.Api.Configuration;

/// <summary>
/// Token issuance and password policy settings, bound from the <c>Authentication</c> section.
/// </summary>
/// <remarks>
/// <see cref="SigningKey"/> has no default and is validated as required, so the application fails
/// fast at start-up rather than falling back to a key embedded in source. Supply it through an
/// environment variable or user-secrets — see docs/setup.md.
/// </remarks>
public sealed class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    [Required(AllowEmptyStrings = false)]
    public string Issuer { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string Audience { get; set; } = string.Empty;

    /// <summary>Symmetric signing key. Must be at least 32 bytes for HMAC-SHA256.</summary>
    [Required(AllowEmptyStrings = false)]
    [MinLength(32, ErrorMessage = "Authentication:SigningKey must be at least 32 characters.")]
    public string SigningKey { get; set; } = string.Empty;

    /// <summary>PROVISIONAL default — the workbook states no session policy. See clarification C-04.</summary>
    [Range(1, 1440)]
    public int AccessTokenLifetimeMinutes { get; set; } = 60;

    /// <summary>PROVISIONAL default — the workbook states no password policy. See clarification C-04.</summary>
    [Range(8, 256)]
    public int MinimumPasswordLength { get; set; } = 12;
}

/// <summary>
/// Fixed-window rate limiting settings, bound from the <c>RateLimiting</c> section.
/// </summary>
/// <remarks>
/// PROVISIONAL defaults. The Excel metric "APIs Without Rate Limiting Count" requires a 429 once a
/// threshold is exceeded but states neither the request count nor the window. See clarification C-05.
/// </remarks>
public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    [Range(1, 100_000)]
    public int PermitLimit { get; set; } = 100;

    [Range(1, 3600)]
    public int WindowSeconds { get; set; } = 60;

    [Range(0, 1000)]
    public int QueueLimit { get; set; }
}

/// <summary>
/// Cross-origin settings, bound from the <c>Cors</c> section.
/// </summary>
public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    /// <summary>Explicit origin allow-list; wildcard origins are never configured.</summary>
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}

/// <summary>
/// One-time administrator provisioning, bound from the <c>Bootstrap</c> section.
/// </summary>
/// <remarks>
/// Both values are optional and absent from every committed configuration file. When they are not
/// supplied no administrator is created, which keeps the repository free of credentials while still
/// allowing a first administrator to be provisioned from the environment.
/// </remarks>
public sealed class BootstrapOptions
{
    public const string SectionName = "Bootstrap";

    [EmailAddress]
    public string? AdministratorEmail { get; set; }

    public string? AdministratorPassword { get; set; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(AdministratorEmail) && !string.IsNullOrWhiteSpace(AdministratorPassword);
}
