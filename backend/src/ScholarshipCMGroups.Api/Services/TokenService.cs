using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Configuration;
using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Services;

/// <summary>
/// Issues signed access tokens for authenticated accounts.
/// </summary>
public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(UserAccount account);
}

/// <summary>Claim types specific to this API.</summary>
public static class ScholarshipClaimTypes
{
    /// <summary>The applicant resource the bearer owns.</summary>
    public const string ApplicantId = "applicant_id";
}

/// <inheritdoc cref="ITokenService"/>
public sealed class TokenService : ITokenService
{
    private readonly AuthenticationOptions _options;
    private readonly IClock _clock;

    public TokenService(IOptions<AuthenticationOptions> options, IClock clock)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public (string Token, DateTimeOffset ExpiresAtUtc) CreateAccessToken(UserAccount account)
    {
        ArgumentNullException.ThrowIfNull(account);

        var expiresAt = _clock.UtcNow.AddMinutes(_options.AccessTokenLifetimeMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: BuildClaims(account),
            notBefore: _clock.UtcNow.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private static List<Claim> BuildClaims(UserAccount account)
    {
        // The subject is the account id, never the email address: the Excel FERPA/GDPR metrics
        // expect no PII to travel in payloads that are also written to logs.
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, Format(account.Id)),
            new(ClaimTypes.Role, account.Role.ToString()),
        };

        if (account.ApplicantId is not null)
        {
            claims.Add(new Claim(ScholarshipClaimTypes.ApplicantId, Format(account.ApplicantId.Value)));
        }

        return claims;
    }

    private static string Format(int value) => value.ToString(CultureInfo.InvariantCulture);
}
