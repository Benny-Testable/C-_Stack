namespace WestCoastFitness.Domain;

public sealed class FitnessAssessment
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public DateOnly AssessedOn { get; set; }

    public int RestingHeartRate { get; set; }

    public decimal BodyMassIndex { get; set; }

    public string Notes { get; set; } = string.Empty;

    public Member? Member { get; set; }
}
