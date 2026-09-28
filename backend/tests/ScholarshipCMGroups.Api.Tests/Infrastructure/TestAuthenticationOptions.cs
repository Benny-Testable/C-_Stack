using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using ScholarshipCMGroups.Api.Configuration;

namespace ScholarshipCMGroups.Api.Tests.Infrastructure;

/// <summary>
/// Authentication settings for tests, with a signing key generated per process.
/// </summary>
/// <remarks>
/// The key is produced at run time by <see cref="RandomNumberGenerator"/> rather than written as a
/// literal, so no key material of any kind — not even a throwaway test key — exists in source
/// control. The Excel "Hardcoded Secret Count" metric scans the whole repository including tests,
/// and its expected value is zero.
/// </remarks>
internal static class TestAuthenticationOptions
{
    /// <summary>48 random bytes, base64 encoded, comfortably above the 32-character minimum.</summary>
    public static string GenerateSigningKey() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

    public static IOptions<AuthenticationOptions> Create(int minimumPasswordLength = 12) =>
        Options.Create(new AuthenticationOptions
        {
            Issuer = "scholarship-cmgroups-tests",
            Audience = "scholarship-cmgroups-tests",
            SigningKey = GenerateSigningKey(),
            AccessTokenLifetimeMinutes = 60,
            MinimumPasswordLength = minimumPasswordLength,
        });
}
