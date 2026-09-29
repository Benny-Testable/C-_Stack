using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public static class SubscriptionRenewalRules
{
    public static RuleDecision Evaluate(Subscription? current, MembershipPlan plan, DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(plan);

        if (!plan.IsActive)
        {
            return RuleDecision.Deny("That membership plan is not offered.");
        }

        if (current is null)
        {
            return RuleDecision.Allow();
        }

        if (current.Status == SubscriptionStatus.Cancelled)
        {
            return RuleDecision.Deny("A cancelled membership cannot be renewed in place. Start a new plan.");
        }

        if (current.Status == SubscriptionStatus.Active && current.EndsOn > today.AddDays(7))
        {
            return RuleDecision.Deny("Renewal opens in the last week of the current term.");
        }

        return RuleDecision.Allow();
    }

    public static (DateOnly StartsOn, DateOnly EndsOn) NextWindow(Subscription? current, MembershipPlan plan, DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(plan);
        var start = current is { Status: SubscriptionStatus.Active } && current.EndsOn >= today
            ? current.EndsOn.AddDays(1)
            : today;
        return (start, start.AddDays(plan.DurationDays));
    }
}
