namespace ScholarshipCMGroups.Api.Models;

/// <summary>
/// Lifecycle states of a scholarship application.
/// </summary>
/// <remarks>
/// PROVISIONAL: the source workbook defines no scholarship workflow. This set is the minimum
/// required to give the Excel "Transition Correctness / Valid Transition Pass Rate" metric a
/// state machine to measure. See docs/clarifications.md item C-02 before relying on it.
/// </remarks>
public enum ApplicationStatus
{
    Draft = 0,
    Submitted = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Withdrawn = 5,
}
