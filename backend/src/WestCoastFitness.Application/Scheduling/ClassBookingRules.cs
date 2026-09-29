using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public static class ClassBookingRules
{
    public static RuleDecision Evaluate(ClassBookingFacts facts)
    {
        ArgumentNullException.ThrowIfNull(facts);

        if (facts.AccountStatus != AccountStatus.Active)
        {
            if (facts.AccountStatus == AccountStatus.Frozen)
            {
                return RuleDecision.Deny("Membership is frozen. Visits resume when the freeze ends.");
            }

            if (facts.AccountStatus == AccountStatus.Pending)
            {
                return RuleDecision.Deny("The account is still pending verification.");
            }

            return RuleDecision.Deny("The member account is closed.");
        }

        if (facts.SubscriptionStatus != SubscriptionStatus.Active || facts.SubscriptionEndsOn is null)
        {
            return RuleDecision.Deny("An active membership is required to book a class.");
        }

        var classDay = DateOnly.FromDateTime(facts.ClassStartsAtUtc);
        if (facts.SubscriptionEndsOn.Value < classDay)
        {
            return RuleDecision.Deny("The membership ends before this class.");
        }

        if (facts.ClassStartsAtUtc <= facts.UtcNow)
        {
            return RuleDecision.Deny("This class has already started.");
        }

        if (facts.ReservedCount >= facts.Capacity)
        {
            return RuleDecision.Deny("This class is at capacity.");
        }

        if (facts.ClassesReservedThisWeek >= facts.MaxClassesPerWeek)
        {
            return RuleDecision.Deny("The weekly class allowance for this plan is already used.");
        }

        if (facts.MemberHasOverlappingReservation)
        {
            return RuleDecision.Deny("Another reservation overlaps this class.");
        }

        if (!facts.TrainerIsActive)
        {
            return RuleDecision.Deny("The assigned trainer is not taking sessions.");
        }

        if (facts.SessionRequiresClearance && !facts.MemberHasMedicalClearance)
        {
            return RuleDecision.Deny("This session requires a current medical clearance.");
        }

        return RuleDecision.Allow();
    }
}
