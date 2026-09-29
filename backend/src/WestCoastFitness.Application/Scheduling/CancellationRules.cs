using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public static class CancellationRules
{
    public static RuleDecision Evaluate(CancellationFacts facts)
    {
        ArgumentNullException.ThrowIfNull(facts);

        if (!facts.BookingExists)
        {
            return RuleDecision.Deny("The reservation was not found.");
        }

        if (!facts.OwnedByMember)
        {
            return RuleDecision.Deny("Only the member who reserved the class can cancel it.");
        }

        if (facts.Status == BookingStatus.Cancelled)
        {
            return RuleDecision.Deny("The reservation is already cancelled.");
        }

        if (facts.Status is BookingStatus.Attended or BookingStatus.NoShow)
        {
            return RuleDecision.Deny("Attendance is already recorded for this reservation.");
        }

        var leadTime = facts.ClassStartsAtUtc - facts.UtcNow;
        if (leadTime < TimeSpan.FromHours(2))
        {
            return RuleDecision.Deny("Cancellations close two hours before the class.");
        }

        return RuleDecision.Allow();
    }
}
