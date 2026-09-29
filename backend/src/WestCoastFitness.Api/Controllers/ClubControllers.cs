using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WestCoastFitness.Application;
using WestCoastFitness.Domain;

namespace WestCoastFitness.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(MemberAccountService accounts) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthSession>> Register([FromBody] RegisterMember command, CancellationToken cancellationToken)
    {
        var session = await accounts.RegisterAsync(command, cancellationToken).ConfigureAwait(false);
        return Ok(session);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthSession>> Login([FromBody] LoginMember command, CancellationToken cancellationToken)
    {
        var session = await accounts.LoginAsync(command, cancellationToken).ConfigureAwait(false);
        return Ok(session);
    }
}

[ApiController]
[Route("api/plans")]
public sealed class PlansController(IMembershipRepository memberships) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<PlanResponse>> List(CancellationToken cancellationToken)
    {
        var plans = await memberships.ListActivePlansAsync(cancellationToken).ConfigureAwait(false);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return plans.Select(plan => new PlanResponse(
            plan.Id,
            plan.Name,
            plan.MonthlyPrice,
            plan.MaxClassesPerWeek,
            SubscriptionService.DescribeForMember(plan, today, today.AddDays(plan.DurationDays)))).ToArray();
    }
}

public sealed record PlanResponse(Guid Id, string Name, decimal MonthlyPrice, int MaxClassesPerWeek, string Window);

[ApiController]
[Authorize]
[Route("api/subscriptions")]
public sealed class SubscriptionsController(SubscriptionService subscriptions) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Subscription>> Open([FromBody] OpenSubscription command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var created = await subscriptions.SubscribeAsync(MemberId(), command.PlanId, command.AutoRenew, cancellationToken).ConfigureAwait(false);
        return Ok(created);
    }

    private Guid MemberId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public sealed record OpenSubscription(Guid PlanId, bool AutoRenew);

[ApiController]
[Authorize]
[Route("api/classes")]
public sealed class ClassesController(IScheduleRepository schedule, ClassBookingService bookings) : ControllerBase
{
    [HttpGet]
    public async Task<IReadOnlyList<ClassSession>> Upcoming(CancellationToken cancellationToken) =>
        await schedule.UpcomingAsync(DateTime.UtcNow, cancellationToken).ConfigureAwait(false);

    [HttpPost("{sessionId:guid}/bookings")]
    public async Task<ActionResult<Booking>> Book(Guid sessionId, CancellationToken cancellationToken)
    {
        var booking = await bookings.BookAsync(MemberId(), sessionId, cancellationToken).ConfigureAwait(false);
        return Ok(booking);
    }

    [HttpPost("bookings/{bookingId:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid bookingId, CancellationToken cancellationToken)
    {
        await bookings.CancelAsync(MemberId(), bookingId, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost]
    public async Task<ActionResult<ClassSession>> Create([FromBody] ClassSession session, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(session);
        session.Id = session.Id == Guid.Empty ? Guid.NewGuid() : session.Id;
        await schedule.AddSessionAsync(session, cancellationToken).ConfigureAwait(false);
        return Ok(session);
    }

    private Guid MemberId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

[ApiController]
[Authorize]
[Route("api/assessments")]
public sealed class AssessmentsController(AssessmentService assessments) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<WorkoutPlan>> Record([FromBody] RecordAssessment command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        var plan = await assessments.RecordAsync(
            MemberId(),
            command.RestingHeartRate,
            command.BodyMassIndex,
            command.Notes,
            cancellationToken).ConfigureAwait(false);
        return Ok(plan);
    }

    private Guid MemberId() => Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public sealed record RecordAssessment(int RestingHeartRate, decimal BodyMassIndex, string Notes);
