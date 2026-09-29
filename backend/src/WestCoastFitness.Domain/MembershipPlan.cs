namespace WestCoastFitness.Domain;

public sealed class MembershipPlan
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal MonthlyPrice { get; set; }

    public int DurationDays { get; set; }

    public int MaxClassesPerWeek { get; set; }

    public bool IsActive { get; set; } = true;
}
