using Microsoft.EntityFrameworkCore;
using WestCoastFitness.Application;
using WestCoastFitness.Domain;

namespace WestCoastFitness.Infrastructure;

public sealed class MemberRepository(FitnessDbContext db) : IMemberRepository
{
    public Task<Member?> FindByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Members.FirstOrDefaultAsync(member => member.Email == email, cancellationToken);

    public Task<Member?> FindByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Members.FirstOrDefaultAsync(member => member.Id == id, cancellationToken);

    public async Task AddAsync(Member member, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(member);
        db.Members.Add(member);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

public sealed class MembershipRepository(FitnessDbContext db) : IMembershipRepository
{
    public async Task<IReadOnlyList<MembershipPlan>> ListActivePlansAsync(CancellationToken cancellationToken) =>
        await db.MembershipPlans.AsNoTracking()
            .Where(plan => plan.IsActive)
            .OrderBy(plan => plan.MonthlyPrice)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public Task<MembershipPlan?> FindPlanAsync(Guid planId, CancellationToken cancellationToken) =>
        db.MembershipPlans.FirstOrDefaultAsync(plan => plan.Id == planId, cancellationToken);

    public Task<Subscription?> FindLatestAsync(Guid memberId, CancellationToken cancellationToken) =>
        db.Subscriptions
            .Where(subscription => subscription.MemberId == memberId)
            .OrderByDescending(subscription => subscription.EndsOn)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task OpenAsync(Subscription subscription, Payment payment, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(subscription);
        ArgumentNullException.ThrowIfNull(payment);
        db.Subscriptions.Add(subscription);
        db.Payments.Add(payment);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}

public sealed class ScheduleRepository(FitnessDbContext db) : IScheduleRepository
{
    public async Task<IReadOnlyList<ClassSession>> UpcomingAsync(DateTime utcNow, CancellationToken cancellationToken) =>
        await db.ClassSessions.AsNoTracking()
            .Include(session => session.Trainer)
            .Where(session => session.StartsAtUtc > utcNow)
            .OrderBy(session => session.StartsAtUtc)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<ClassBookingFacts?> LoadBookingFactsAsync(Guid memberId, Guid sessionId, CancellationToken cancellationToken)
    {
        var member = await db.Members.AsNoTracking().FirstOrDefaultAsync(row => row.Id == memberId, cancellationToken).ConfigureAwait(false);
        var session = await db.ClassSessions.AsNoTracking()
            .Include(row => row.Trainer)
            .FirstOrDefaultAsync(row => row.Id == sessionId, cancellationToken)
            .ConfigureAwait(false);
        if (member is null || session is null)
        {
            return null;
        }

        var subscription = await db.Subscriptions.AsNoTracking()
            .Include(row => row.Plan)
            .Where(row => row.MemberId == memberId)
            .OrderByDescending(row => row.EndsOn)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        var reservedCount = await db.Bookings.CountAsync(
            row => row.ClassSessionId == sessionId && row.Status == BookingStatus.Reserved,
            cancellationToken).ConfigureAwait(false);

        var weekStart = WeekStart(session.StartsAtUtc);
        var weekEnd = weekStart.AddDays(7);
        var weekCount = await db.Bookings.CountAsync(
            row => row.MemberId == memberId
                && row.Status == BookingStatus.Reserved
                && row.ClassSession != null
                && row.ClassSession.StartsAtUtc >= weekStart
                && row.ClassSession.StartsAtUtc < weekEnd,
            cancellationToken).ConfigureAwait(false);

        var sessionEnd = session.StartsAtUtc.AddMinutes(session.DurationMinutes);
        var overlap = await db.Bookings.AnyAsync(
            row => row.MemberId == memberId
                && row.Status == BookingStatus.Reserved
                && row.ClassSession != null
                && row.ClassSession.StartsAtUtc < sessionEnd
                && row.ClassSession.StartsAtUtc.AddMinutes(row.ClassSession.DurationMinutes) > session.StartsAtUtc,
            cancellationToken).ConfigureAwait(false);

        return new ClassBookingFacts(
            member.Status,
            subscription?.Status,
            subscription?.EndsOn,
            session.StartsAtUtc,
            DateTime.UtcNow,
            reservedCount,
            session.Capacity,
            weekCount,
            subscription?.Plan?.MaxClassesPerWeek ?? 0,
            overlap,
            session.Trainer?.IsActive ?? false,
            session.RequiresMedicalClearance,
            member.HasMedicalClearance);
    }

    public async Task<CancellationFacts?> LoadCancellationFactsAsync(Guid memberId, Guid bookingId, CancellationToken cancellationToken)
    {
        var booking = await db.Bookings.AsNoTracking()
            .Include(row => row.ClassSession)
            .FirstOrDefaultAsync(row => row.Id == bookingId, cancellationToken)
            .ConfigureAwait(false);
        if (booking?.ClassSession is null)
        {
            return new CancellationFacts(false, false, BookingStatus.Cancelled, DateTime.UtcNow, DateTime.UtcNow);
        }

        return new CancellationFacts(
            true,
            booking.MemberId == memberId,
            booking.Status,
            booking.ClassSession.StartsAtUtc,
            DateTime.UtcNow);
    }

    public async Task<Booking> AddBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(booking);
        db.Bookings.Add(booking);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return booking;
    }

    public async Task CancelAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        var booking = await db.Bookings.FirstAsync(row => row.Id == bookingId, cancellationToken).ConfigureAwait(false);
        booking.Status = BookingStatus.Cancelled;
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task AddSessionAsync(ClassSession session, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);
        db.ClassSessions.Add(session);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static DateTime WeekStart(DateTime instant)
    {
        var date = instant.Date;
        var delta = ((int)date.DayOfWeek + 6) % 7;
        return DateTime.SpecifyKind(date.AddDays(-delta), DateTimeKind.Utc);
    }
}

public sealed class AssessmentRepository(FitnessDbContext db) : IAssessmentRepository
{
    public async Task AddAsync(FitnessAssessment assessment, WorkoutPlan plan, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(assessment);
        ArgumentNullException.ThrowIfNull(plan);
        db.FitnessAssessments.Add(assessment);
        db.WorkoutPlans.Add(plan);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<FitnessAssessment>> ListForMemberAsync(Guid memberId, CancellationToken cancellationToken) =>
        await db.FitnessAssessments.AsNoTracking()
            .Where(row => row.MemberId == memberId)
            .OrderBy(row => row.AssessedOn)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
