namespace WestCoastFitness.Domain;

public sealed class Booking
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public Guid ClassSessionId { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Reserved;

    public DateTime BookedAtUtc { get; set; }

    public Member? Member { get; set; }

    public ClassSession? ClassSession { get; set; }
}
