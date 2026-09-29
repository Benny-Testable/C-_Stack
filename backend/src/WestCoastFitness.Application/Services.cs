using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public sealed record RegisterMember(string Email, string FullName, string Password);

public sealed record LoginMember(string Email, string Password);

public sealed record AuthSession(Guid MemberId, string Email, string FullName, string Role, string Token);

public sealed class MemberAccountService(
    IMemberRepository members,
    IPasswordHasher passwords,
    ITokenIssuer tokens,
    IClock clock)
{
    public async Task<AuthSession> RegisterAsync(RegisterMember command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var email = command.Email.Trim();
        var name = command.FullName.Trim();
        var error = MemberRegistrationRules.Validate(email, name, command.Password);
        if (error is not null)
        {
            throw new ClubRuleException(error);
        }

        if (await members.FindByEmailAsync(email, cancellationToken).ConfigureAwait(false) is not null)
        {
            throw new ClubRuleException("An account with that email already exists.");
        }

        var member = new Member
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = name,
            PasswordHash = passwords.Hash(command.Password),
            Role = MemberRole.Member,
            Status = AccountStatus.Active,
            CreatedAtUtc = clock.UtcNow,
        };
        await members.AddAsync(member, cancellationToken).ConfigureAwait(false);
        return ToSession(member);
    }

    public async Task<AuthSession> LoginAsync(LoginMember command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var member = await members.FindByEmailAsync(command.Email.Trim(), cancellationToken).ConfigureAwait(false);
        if (member is null || !passwords.Verify(member.PasswordHash, command.Password))
        {
            throw new ClubRuleException("Email or password is incorrect.");
        }

        if (member.Status != AccountStatus.Active)
        {
            throw new ClubRuleException("This account cannot sign in.");
        }

        return ToSession(member);
    }

    private AuthSession ToSession(Member member) =>
        new(member.Id, member.Email, member.FullName, member.Role.ToString(), tokens.Issue(member));
}

public sealed class SubscriptionService(
    IMembershipRepository memberships,
    IPaymentGateway payments,
    IClock clock)
{
    public async Task<Subscription> SubscribeAsync(Guid memberId, Guid planId, bool autoRenew, CancellationToken cancellationToken)
    {
        var plan = await memberships.FindPlanAsync(planId, cancellationToken).ConfigureAwait(false)
            ?? throw new ClubRuleException("Membership plan was not found.");
        var current = await memberships.FindLatestAsync(memberId, cancellationToken).ConfigureAwait(false);
        var today = DateOnly.FromDateTime(clock.UtcNow);
        var decision = SubscriptionRenewalRules.Evaluate(current, plan, today);
        if (!decision.Allowed)
        {
            throw new ClubRuleException(decision.Reason);
        }

        var capture = await payments.CaptureAsync(plan.MonthlyPrice, plan.Name, cancellationToken).ConfigureAwait(false);
        if (!capture.Succeeded)
        {
            throw new ClubRuleException(capture.DeclineReason);
        }

        var window = SubscriptionRenewalRules.NextWindow(current, plan, today);
        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            PlanId = plan.Id,
            StartsOn = window.StartsOn,
            EndsOn = window.EndsOn,
            Status = SubscriptionStatus.Active,
            AutoRenew = autoRenew,
        };
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            SubscriptionId = subscription.Id,
            Amount = plan.MonthlyPrice,
            Status = PaymentStatus.Captured,
            ProcessedAtUtc = clock.UtcNow,
            ProviderReference = capture.ProviderReference,
        };
        await memberships.OpenAsync(subscription, payment, cancellationToken).ConfigureAwait(false);
        return subscription;
    }

    public static string DescribeForMember(MembershipPlan plan, DateOnly startsOn, DateOnly endsOn)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return MembershipWindowText.Describe(plan.Name, startsOn, endsOn, plan.MaxClassesPerWeek, plan.MonthlyPrice);
    }

    public static string DescribeForStaff(MembershipPlan plan, DateOnly startsOn, DateOnly endsOn)
    {
        ArgumentNullException.ThrowIfNull(plan);
        return StaffMembershipWindowText.Describe(plan.Name, startsOn, endsOn, plan.MaxClassesPerWeek, plan.MonthlyPrice);
    }
}

public sealed class ClassBookingService(IScheduleRepository schedule, IClock clock)
{
    public async Task<Booking> BookAsync(Guid memberId, Guid sessionId, CancellationToken cancellationToken)
    {
        var facts = await schedule.LoadBookingFactsAsync(memberId, sessionId, cancellationToken).ConfigureAwait(false)
            ?? throw new ClubRuleException("The class or member was not found.");
        var decision = ClassBookingRules.Evaluate(facts);
        if (!decision.Allowed)
        {
            throw new ClubRuleException(decision.Reason);
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            ClassSessionId = sessionId,
            Status = BookingStatus.Reserved,
            BookedAtUtc = clock.UtcNow,
        };
        return await schedule.AddBookingAsync(booking, cancellationToken).ConfigureAwait(false);
    }

    public async Task CancelAsync(Guid memberId, Guid bookingId, CancellationToken cancellationToken)
    {
        var facts = await schedule.LoadCancellationFactsAsync(memberId, bookingId, cancellationToken).ConfigureAwait(false)
            ?? throw new ClubRuleException("The reservation was not found.");
        var decision = CancellationRules.Evaluate(facts);
        if (!decision.Allowed)
        {
            throw new ClubRuleException(decision.Reason);
        }

        await schedule.CancelAsync(bookingId, cancellationToken).ConfigureAwait(false);
    }
}

public sealed class AssessmentService(IAssessmentRepository assessments, IClock clock)
{
    public async Task<WorkoutPlan> RecordAsync(
        Guid memberId,
        int restingHeartRate,
        decimal bodyMassIndex,
        string notes,
        CancellationToken cancellationToken)
    {
        if (restingHeartRate is < 30 or > 220)
        {
            throw new ClubRuleException("Resting heart rate is outside the recordable range.");
        }

        if (bodyMassIndex is < 10m or > 80m)
        {
            throw new ClubRuleException("Body mass index is outside the recordable range.");
        }

        ArgumentNullException.ThrowIfNull(notes);
        var today = DateOnly.FromDateTime(clock.UtcNow);
        var history = await assessments.ListForMemberAsync(memberId, cancellationToken).ConfigureAwait(false);
        var assessment = new FitnessAssessment
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            AssessedOn = today,
            RestingHeartRate = restingHeartRate,
            BodyMassIndex = bodyMassIndex,
            Notes = notes.Trim(),
        };
        var combined = new List<FitnessAssessment>(history) { assessment };
        var plan = WorkoutPlanAdvisor.Recommend(memberId, combined, today);
        await assessments.AddAsync(assessment, plan, cancellationToken).ConfigureAwait(false);
        return plan;
    }
}
