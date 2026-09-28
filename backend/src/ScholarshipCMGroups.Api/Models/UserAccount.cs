namespace ScholarshipCMGroups.Api.Models;

/// <summary>
/// A credential record used to authenticate callers of the API.
/// </summary>
public class UserAccount
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    /// <summary>Output of <see cref="Microsoft.AspNetCore.Identity.PasswordHasher{TUser}"/>; never a plaintext password.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Applicant;

    /// <summary>Set for <see cref="UserRole.Applicant"/> accounts; the resource they own.</summary>
    public int? ApplicantId { get; set; }

    public Applicant? Applicant { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; }
}
