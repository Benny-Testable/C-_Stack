using WestCoastFitness.Application;
using WestCoastFitness.Domain;

namespace WestCoastFitness.Application.Tests;

public class ClubRulesTests
{
    private static ClassBookingFacts Facts(
        AccountStatus account = AccountStatus.Active,
        SubscriptionStatus? subscription = SubscriptionStatus.Active,
        DateOnly? endsOn = null,
        DateTime? classStart = null,
        int reserved = 1,
        int capacity = 16,
        int weekCount = 0,
        int maxPerWeek = 5,
        bool overlap = false,
        bool trainerActive = true,
        bool needsClearance = false,
        bool hasClearance = false)
    {
        var start = classStart ?? new DateTime(2027, 3, 2, 15, 0, 0, DateTimeKind.Utc);
        return new ClassBookingFacts(
            account,
            subscription,
            endsOn ?? new DateOnly(2027, 4, 1),
            start,
            new DateTime(2027, 3, 1, 12, 0, 0, DateTimeKind.Utc),
            reserved,
            capacity,
            weekCount,
            maxPerWeek,
            overlap,
            trainerActive,
            needsClearance,
            hasClearance);
    }

    [Fact]
    public void Booking_allows_an_active_member_inside_plan_limits()
    {
        var decision = ClassBookingRules.Evaluate(Facts());
        Assert.True(decision.Allowed);
    }

    [Theory]
    [InlineData(AccountStatus.Frozen, "frozen")]
    [InlineData(AccountStatus.Pending, "pending")]
    [InlineData(AccountStatus.Closed, "closed")]
    public void Booking_denies_inactive_accounts(AccountStatus status, string expected)
    {
        var decision = ClassBookingRules.Evaluate(Facts(account: status));
        Assert.False(decision.Allowed);
        Assert.Contains(expected, decision.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Booking_denies_a_full_class_and_a_weekly_cap_and_an_overlap()
    {
        Assert.Contains("capacity", ClassBookingRules.Evaluate(Facts(reserved: 16, capacity: 16)).Reason, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("weekly", ClassBookingRules.Evaluate(Facts(weekCount: 5, maxPerWeek: 5)).Reason, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("overlaps", ClassBookingRules.Evaluate(Facts(overlap: true)).Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Booking_denies_clearance_and_inactive_trainers()
    {
        Assert.Contains("clearance", ClassBookingRules.Evaluate(Facts(needsClearance: true)).Reason, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("trainer", ClassBookingRules.Evaluate(Facts(trainerActive: false)).Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cancellation_closes_inside_two_hours()
    {
        var start = new DateTime(2027, 3, 2, 15, 0, 0, DateTimeKind.Utc);
        var decision = CancellationRules.Evaluate(new CancellationFacts(true, true, BookingStatus.Reserved, start, start.AddHours(-1)));
        Assert.False(decision.Allowed);
        Assert.Contains("two hours", decision.Reason, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cancellation_allows_a_day_ahead()
    {
        var start = new DateTime(2027, 3, 2, 15, 0, 0, DateTimeKind.Utc);
        var decision = CancellationRules.Evaluate(new CancellationFacts(true, true, BookingStatus.Reserved, start, start.AddDays(-1)));
        Assert.True(decision.Allowed);
    }

    [Fact]
    public void Registration_rejects_a_short_password_and_accepts_a_mixed_one()
    {
        Assert.NotNull(MemberRegistrationRules.Validate("member@westcoast.example", "Avery Stone", "short"));
        Assert.Null(MemberRegistrationRules.Validate("member@westcoast.example", "Avery Stone", "DesertGym10"));
    }

    [Fact]
    public void Renewal_waits_until_the_final_week()
    {
        var plan = new MembershipPlan { Name = "Canyon Peak", IsActive = true, DurationDays = 30, MaxClassesPerWeek = 5, MonthlyPrice = 79m };
        var current = new Subscription
        {
            Status = SubscriptionStatus.Active,
            EndsOn = new DateOnly(2027, 4, 20),
        };
        var decision = SubscriptionRenewalRules.Evaluate(current, plan, new DateOnly(2027, 3, 1));
        Assert.False(decision.Allowed);
    }

    [Fact]
    public void Advisor_lowers_volume_when_heart_rate_and_bmi_are_both_high()
    {
        var history = new List<FitnessAssessment>
        {
            new()
            {
                AssessedOn = new DateOnly(2027, 1, 2),
                RestingHeartRate = 88,
                BodyMassIndex = 32m,
            },
        };
        var plan = WorkoutPlanAdvisor.Recommend(Guid.NewGuid(), history, new DateOnly(2027, 1, 3));
        Assert.Equal(2, plan.SessionsPerWeek);
        Assert.Contains("Recovery", plan.Title, StringComparison.Ordinal);
    }

    [Fact]
    public void Attendance_risk_rises_on_no_shows_and_falls_on_visits()
    {
        var score = AttendanceRisk.Score([BookingStatus.NoShow, BookingStatus.NoShow, BookingStatus.Attended]);
        Assert.Equal(5, score);
    }

    [Fact]
    public void Member_and_staff_window_text_share_the_allowance_and_differ_by_audience()
    {
        var start = new DateOnly(2027, 3, 1);
        var end = new DateOnly(2027, 3, 31);
        var member = MembershipWindowText.Describe("Desert Dawn", start, end, 3, 49m);
        var staff = StaffMembershipWindowText.Describe("Desert Dawn", start, end, 3, 49m);
        Assert.Contains("included class visits", member, StringComparison.Ordinal);
        Assert.Contains("included class visits", staff, StringComparison.Ordinal);
        Assert.Contains("member", member, StringComparison.Ordinal);
        Assert.Contains("front desk", staff, StringComparison.Ordinal);
    }
}
