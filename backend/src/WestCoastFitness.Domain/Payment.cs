namespace WestCoastFitness.Domain;

public sealed class Payment
{
    public Guid Id { get; set; }

    public Guid SubscriptionId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; set; }

    public DateTime ProcessedAtUtc { get; set; }

    public string ProviderReference { get; set; } = string.Empty;

    public Subscription? Subscription { get; set; }
}
