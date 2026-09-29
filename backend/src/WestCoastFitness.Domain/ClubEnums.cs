namespace WestCoastFitness.Domain;

public enum MemberRole
{
    Member = 0,
    Trainer = 1,
    Administrator = 2,
}

public enum AccountStatus
{
    Pending = 0,
    Active = 1,
    Frozen = 2,
    Closed = 3,
}

public enum SubscriptionStatus
{
    PendingPayment = 0,
    Active = 1,
    Expired = 2,
    Cancelled = 3,
}

public enum BookingStatus
{
    Reserved = 0,
    Cancelled = 1,
    Attended = 2,
    NoShow = 3,
}

public enum PaymentStatus
{
    Pending = 0,
    Captured = 1,
    Declined = 2,
    Refunded = 3,
}
