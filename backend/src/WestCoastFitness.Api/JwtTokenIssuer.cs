using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WestCoastFitness.Application;
using WestCoastFitness.Domain;

namespace WestCoastFitness.Api;

public sealed class JwtTokenIssuer(IConfiguration configuration) : ITokenIssuer
{
    public string Issue(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        var keyText = configuration["Jwt:SigningKey"] ?? string.Empty;
        if (keyText.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SigningKey must be at least 32 characters.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, member.Id.ToString()),
            new(ClaimTypes.Role, member.Role.ToString()),
            new(JwtRegisteredClaimNames.Email, member.Email),
        };
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
