using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Services;

namespace ScholarshipCMGroups.Api.Controllers;

/// <summary>
/// Applicant profiles.
/// </summary>
[Route("api/applicants")]
[Authorize]
public sealed class ApplicantsController : ApiControllerBase
{
    private readonly IApplicantService _applicants;
    private readonly ILogger<ApplicantsController> _logger;

    public ApplicantsController(IApplicantService applicants, ILogger<ApplicantsController> logger)
    {
        _applicants = applicants ?? throw new ArgumentNullException(nameof(applicants));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>Gets an applicant profile. Applicants may read only their own record.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicantResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        if (!GetCaller().CanAccessApplicant(id))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return ToActionResult(await _applicants.GetByIdAsync(id, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>Creates an applicant profile without a login, for administrator-led intake.</summary>
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicantResponse>> Create(
        [FromBody] ApplicantRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _applicants.CreateAsync(request, cancellationToken).ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Updates an applicant profile. Applicants may update only their own record.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicantResponse>> Update(
        int id,
        [FromBody] ApplicantRequest request,
        CancellationToken cancellationToken)
    {
        if (!GetCaller().CanAccessApplicant(id))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        return ToActionResult(await _applicants.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false));
    }

    /// <summary>
    /// Erases an applicant and every record referencing them.
    /// </summary>
    /// <remarks>
    /// Evidence endpoint for the Excel "Data Deletion Verification Rate" metric. The receipt states
    /// how many dependent rows were removed so a test can assert the erasure reached every layer.
    /// </remarks>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ErasureReceipt>> Erase(int id, CancellationToken cancellationToken)
    {
        if (!GetCaller().CanAccessApplicant(id))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _applicants.EraseAsync(id, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            // Logs the surrogate key only; the applicant's name and email are never written to logs,
            // which the Excel "PII Log Statement Count" metric expects to be zero.
            LogMessages.ApplicantErased(_logger, id, result.Value!.ApplicationsDeleted);
        }

        return ToActionResult(result);
    }
}
