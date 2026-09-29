using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public sealed record ClassBookingFacts(
    AccountStatus AccountStatus,
    SubscriptionStatus? SubscriptionStatus,
    DateOnly? SubscriptionEndsOn,
    DateTime ClassStartsAtUtc,
    DateTime UtcNow,
    int ReservedCount,
    int Capacity,
    int ClassesReservedThisWeek,
    int MaxClassesPerWeek,
    bool MemberHasOverlappingReservation,
    bool TrainerIsActive,
    bool SessionRequiresClearance,
    bool MemberHasMedicalClearance);

public sealed record CancellationFacts(
    bool BookingExists,
    bool OwnedByMember,
    BookingStatus Status,
    DateTime ClassStartsAtUtc,
    DateTime UtcNow);

public sealed record RuleDecision(bool Allowed, string Reason)
{
    public static RuleDecision Allow() => new(true, "Allowed.");

    public static RuleDecision Deny(string reason) => new(false, reason);
}
