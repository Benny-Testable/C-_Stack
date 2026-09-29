namespace WestCoastFitness.Domain;

public sealed class ClassSession
{
    public Guid Id { get; set; }

    public Guid TrainerId { get; set; }

    public string Title { get; set; } = string.Empty;

    public DateTime StartsAtUtc { get; set; }

    public int DurationMinutes { get; set; }

    public int Capacity { get; set; }

    public bool RequiresMedicalClearance { get; set; }

    public Trainer? Trainer { get; set; }
}
