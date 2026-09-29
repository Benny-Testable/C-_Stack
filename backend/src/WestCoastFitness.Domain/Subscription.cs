namespace WestCoastFitness.Domain;

public sealed class Subscription
{
    public Guid Id { get; set; }

    public Guid MemberId { get; set; }

    public Guid PlanId { get; set; }

    public DateOnly StartsOn { get; set; }

    public DateOnly EndsOn { get; set; }

    public SubscriptionStatus Status { get; set; }

    public bool AutoRenew { get; set; }

    public Member? Member { get; set; }

    public MembershipPlan? Plan { get; set; }
}
