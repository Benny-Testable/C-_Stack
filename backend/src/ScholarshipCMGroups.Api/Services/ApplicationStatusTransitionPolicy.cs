using ScholarshipCMGroups.Api.Models;

namespace ScholarshipCMGroups.Api.Services;

/// <summary>
/// Decides which application lifecycle transitions are legal.
/// </summary>
public interface IApplicationStatusTransitionPolicy
{
    /// <summary>Returns the states reachable in one step from <paramref name="current"/>.</summary>
    IReadOnlyCollection<ApplicationStatus> AllowedTransitionsFrom(ApplicationStatus current);

    /// <summary>Returns <see langword="true"/> when moving from <paramref name="current"/> to <paramref name="target"/> is legal.</summary>
    bool IsTransitionAllowed(ApplicationStatus current, ApplicationStatus target);

    /// <summary>Returns <see langword="true"/> when no transition out of <paramref name="status"/> exists.</summary>
    bool IsTerminal(ApplicationStatus status);
}

/// <summary>
/// Table-driven implementation of the application state machine.
/// </summary>
/// <remarks>
/// PROVISIONAL: the transition table encodes a workflow that the source workbook does not define.
/// It exists so the Excel "Valid Transition Pass Rate" and "State Transition Accuracy %" metrics
/// have a real state machine to measure. Confirm the workflow before production use — see
/// docs/clarifications.md item C-02.
///
/// Expressed as a lookup rather than nested conditionals so that cyclomatic complexity stays at 1
/// per method, satisfying the Excel expected value of "&lt;= 10 per function".
/// </remarks>
public sealed class ApplicationStatusTransitionPolicy : IApplicationStatusTransitionPolicy
{
    private static readonly Dictionary<ApplicationStatus, ApplicationStatus[]> Transitions =
        new()
        {
            [ApplicationStatus.Draft] = new[] { ApplicationStatus.Submitted, ApplicationStatus.Withdrawn },
            [ApplicationStatus.Submitted] = new[] { ApplicationStatus.UnderReview, ApplicationStatus.Withdrawn },
            [ApplicationStatus.UnderReview] = new[]
            {
                ApplicationStatus.Approved,
                ApplicationStatus.Rejected,
                ApplicationStatus.Withdrawn,
            },
            [ApplicationStatus.Approved] = Array.Empty<ApplicationStatus>(),
            [ApplicationStatus.Rejected] = Array.Empty<ApplicationStatus>(),
            [ApplicationStatus.Withdrawn] = Array.Empty<ApplicationStatus>(),
        };

    public IReadOnlyCollection<ApplicationStatus> AllowedTransitionsFrom(ApplicationStatus current) =>
        Transitions.TryGetValue(current, out var allowed) ? allowed : Array.Empty<ApplicationStatus>();

    public bool IsTransitionAllowed(ApplicationStatus current, ApplicationStatus target) =>
        AllowedTransitionsFrom(current).Contains(target);

    public bool IsTerminal(ApplicationStatus status) => AllowedTransitionsFrom(status).Count == 0;
}
