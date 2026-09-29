namespace WestCoastFitness.Domain;

public sealed class WorkoutPlan
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public string Title { get; set; } = string.Empty;

    public int SessionsPerWeek { get; set; }

    public DateOnly CreatedOn { get; set; }

    public Member? Member { get; set; }
}
