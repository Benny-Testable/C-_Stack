namespace WestCoastFitness.Domain;

public sealed class Member
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public MemberRole Role { get; set; } = MemberRole.Member;

    public AccountStatus Status { get; set; } = AccountStatus.Active;

    public bool HasMedicalClearance { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
