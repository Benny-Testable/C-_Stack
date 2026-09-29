using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public interface IClock
{
    DateTime UtcNow { get; }
}

public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string passwordHash, string password);
}

public interface ITokenIssuer
{
    string Issue(Member member);
}

public sealed record PaymentCapture(bool Succeeded, string ProviderReference, string DeclineReason);

public interface IPaymentGateway
{
    Task<PaymentCapture> CaptureAsync(decimal amount, string reference, CancellationToken cancellationToken);
}

public interface IMemberRepository
{
    Task<Member?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    Task<Member?> FindByIdAsync(Guid id, CancellationToken cancellationToken);

    Task AddAsync(Member member, CancellationToken cancellationToken);
}

public interface IMembershipRepository
{
    Task<IReadOnlyList<MembershipPlan>> ListActivePlansAsync(CancellationToken cancellationToken);

    Task<MembershipPlan?> FindPlanAsync(Guid planId, CancellationToken cancellationToken);

    Task<Subscription?> FindLatestAsync(Guid memberId, CancellationToken cancellationToken);

    Task OpenAsync(Subscription subscription, Payment payment, CancellationToken cancellationToken);
}

public interface IScheduleRepository
{
    Task<IReadOnlyList<ClassSession>> UpcomingAsync(DateTime utcNow, CancellationToken cancellationToken);

    Task<ClassBookingFacts?> LoadBookingFactsAsync(Guid memberId, Guid sessionId, CancellationToken cancellationToken);

    Task<CancellationFacts?> LoadCancellationFactsAsync(Guid memberId, Guid bookingId, CancellationToken cancellationToken);

    Task<Booking> AddBookingAsync(Booking booking, CancellationToken cancellationToken);

    Task CancelAsync(Guid bookingId, CancellationToken cancellationToken);

    Task AddSessionAsync(ClassSession session, CancellationToken cancellationToken);
}

public interface IAssessmentRepository
{
    Task AddAsync(FitnessAssessment assessment, WorkoutPlan plan, CancellationToken cancellationToken);

    Task<IReadOnlyList<FitnessAssessment>> ListForMemberAsync(Guid memberId, CancellationToken cancellationToken);
}
