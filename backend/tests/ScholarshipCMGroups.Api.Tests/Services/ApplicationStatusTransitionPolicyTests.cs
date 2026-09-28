using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Services;
using Xunit;

namespace ScholarshipCMGroups.Api.Tests.Services;

/// <summary>
/// Exhaustive check of the application state machine.
/// </summary>
/// <remarks>
/// The theory below enumerates every ordered pair of statuses, so the transition table is covered
/// completely rather than sampled. This is the evidence for the Excel "State Transition Accuracy %"
/// metric and for the state-machine branch of "Decision Coverage".
/// </remarks>
public sealed class ApplicationStatusTransitionPolicyTests
{
    private static readonly (ApplicationStatus From, ApplicationStatus To)[] LegalTransitions =
    {
        (ApplicationStatus.Draft, ApplicationStatus.Submitted),
        (ApplicationStatus.Draft, ApplicationStatus.Withdrawn),
        (ApplicationStatus.Submitted, ApplicationStatus.UnderReview),
        (ApplicationStatus.Submitted, ApplicationStatus.Withdrawn),
        (ApplicationStatus.UnderReview, ApplicationStatus.Approved),
        (ApplicationStatus.UnderReview, ApplicationStatus.Rejected),
        (ApplicationStatus.UnderReview, ApplicationStatus.Withdrawn),
    };

    private readonly ApplicationStatusTransitionPolicy _policy = new();

    public static TheoryData<ApplicationStatus, ApplicationStatus> AllStatusPairs()
    {
        var data = new TheoryData<ApplicationStatus, ApplicationStatus>();

        foreach (var from in Enum.GetValues<ApplicationStatus>())
        {
            foreach (var to in Enum.GetValues<ApplicationStatus>())
            {
                data.Add(from, to);
            }
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(AllStatusPairs))]
    public void IsTransitionAllowed_matches_the_declared_transition_table(
        ApplicationStatus from,
        ApplicationStatus to)
    {
        var expected = LegalTransitions.Contains((from, to));

        Assert.Equal(expected, _policy.IsTransitionAllowed(from, to));
    }

    [Theory]
    [InlineData(ApplicationStatus.Approved)]
    [InlineData(ApplicationStatus.Rejected)]
    [InlineData(ApplicationStatus.Withdrawn)]
    public void Decided_states_are_terminal(ApplicationStatus status)
    {
        Assert.True(_policy.IsTerminal(status));
        Assert.Empty(_policy.AllowedTransitionsFrom(status));
    }

    [Theory]
    [InlineData(ApplicationStatus.Draft)]
    [InlineData(ApplicationStatus.Submitted)]
    [InlineData(ApplicationStatus.UnderReview)]
    public void In_flight_states_are_not_terminal(ApplicationStatus status)
    {
        Assert.False(_policy.IsTerminal(status));
        Assert.NotEmpty(_policy.AllowedTransitionsFrom(status));
    }

    [Fact]
    public void No_status_can_transition_to_itself()
    {
        foreach (var status in Enum.GetValues<ApplicationStatus>())
        {
            Assert.False(_policy.IsTransitionAllowed(status, status));
        }
    }

    [Fact]
    public void A_status_outside_the_enum_range_yields_no_transitions()
    {
        // Guards the TryGetValue fallback: an unmapped value must produce an empty set rather than
        // throwing, so a future enum member cannot crash the pipeline before its row is added.
        const ApplicationStatus Unmapped = (ApplicationStatus)9999;

        Assert.Empty(_policy.AllowedTransitionsFrom(Unmapped));
        Assert.True(_policy.IsTerminal(Unmapped));
    }
}
