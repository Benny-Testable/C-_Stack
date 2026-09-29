namespace WestCoastFitness.Domain;

public sealed class AttendanceRecord
{
    public Guid Id { get; set; }

    public Guid BookingId { get; set; }

    public DateTime CheckedInAtUtc { get; set; }

    public BookingStatus Outcome { get; set; }

    public Booking? Booking { get; set; }
}
