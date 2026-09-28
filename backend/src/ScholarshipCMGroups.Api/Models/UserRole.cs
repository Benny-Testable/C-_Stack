namespace ScholarshipCMGroups.Api.Models;

/// <summary>
/// Authorization roles recognised by the API.
/// </summary>
/// <remarks>
/// PROVISIONAL: the source workbook names no user roles. These two exist only because the
/// Excel security metrics ("Unauthenticated API Endpoint Count", "BOLA Finding Count") require
/// an authenticated caller with a distinguishable privilege level. See docs/clarifications.md
/// item C-03.
/// </remarks>
public enum UserRole
{
    Applicant = 0,
    Administrator = 1,
}
