using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Services;

namespace ScholarshipCMGroups.Api.Controllers;

/// <summary>
/// Scholarship application lifecycle.
/// </summary>
[Route("api/applications")]
[Authorize]
public sealed class ApplicationsController : ApiControllerBase
{
    private readonly IScholarshipApplicationService _applications;

    public ApplicationsController(IScholarshipApplicationService applications)
    {
        _applications = applications ?? throw new ArgumentNullException(nameof(applications));
    }

    /// <summary>Lists applications. Applicants see only their own; administrators see all.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ApplicationResponse>>> GetPage(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] int? scholarshipId,
        [FromQuery] ApplicationStatus? status,
        CancellationToken cancellationToken)
    {
        var result = await _applications
            .GetPageAsync(GetCaller(), page, pageSize, scholarshipId, status, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>Gets one application by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationResponse>> GetById(int id, CancellationToken cancellationToken) =>
        ToActionResult(await _applications.GetByIdAsync(GetCaller(), id, cancellationToken).ConfigureAwait(false));

    /// <summary>Opens a draft application against a scholarship.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ApplicationResponse>> Create(
        [FromBody] ApplicationCreateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _applications
            .CreateDraftAsync(GetCaller(), request, cancellationToken)
            .ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Edits the motivation statement of a draft application.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationResponse>> Update(
        int id,
        [FromBody] ApplicationUpdateRequest request,
        CancellationToken cancellationToken) =>
        ToActionResult(await _applications
            .UpdateDraftAsync(GetCaller(), id, request, cancellationToken)
            .ConfigureAwait(false));

    /// <summary>
    /// Moves an application to a new lifecycle state.
    /// </summary>
    /// <remarks>
    /// Rejects any transition the state machine does not permit, which is the evidence path for the
    /// Excel "Valid Transition Pass Rate" and "State Transition Accuracy %" metrics.
    /// </remarks>
    [HttpPost("{id:int}/transitions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationResponse>> Transition(
        int id,
        [FromBody] ApplicationTransitionRequest request,
        CancellationToken cancellationToken) =>
        ToActionResult(await _applications
            .TransitionAsync(GetCaller(), id, request, cancellationToken)
            .ConfigureAwait(false));
}
