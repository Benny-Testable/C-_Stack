using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Api.Common;
using ScholarshipCMGroups.Api.Contracts;
using ScholarshipCMGroups.Api.Models;
using ScholarshipCMGroups.Api.Services;

namespace ScholarshipCMGroups.Api.Controllers;

/// <summary>
/// Scholarship programme catalogue.
/// </summary>
[Route("api/scholarships")]
[Authorize]
public sealed class ScholarshipsController : ApiControllerBase
{
    private readonly IScholarshipService _scholarships;
    private readonly IScholarshipApplicationService _applications;

    public ScholarshipsController(
        IScholarshipService scholarships,
        IScholarshipApplicationService applications)
    {
        _scholarships = scholarships ?? throw new ArgumentNullException(nameof(scholarships));
        _applications = applications ?? throw new ArgumentNullException(nameof(applications));
    }

    /// <summary>Lists scholarships. Open to anonymous callers so the catalogue can be browsed before sign-up.</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ScholarshipResponse>>> GetPage(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] bool? activeOnly,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var result = await _scholarships
            .GetPageAsync(page, pageSize, activeOnly, search, cancellationToken)
            .ConfigureAwait(false);

        return Ok(result);
    }

    /// <summary>Gets one scholarship by id.</summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScholarshipResponse>> GetById(int id, CancellationToken cancellationToken) =>
        ToActionResult(await _scholarships.GetByIdAsync(id, cancellationToken).ConfigureAwait(false));

    /// <summary>Reports observed application counts for one scholarship.</summary>
    [HttpGet("{id:int}/statistics")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScholarshipStatisticsResponse>> GetStatistics(
        int id,
        CancellationToken cancellationToken) =>
        ToActionResult(await _applications.GetStatisticsAsync(id, cancellationToken).ConfigureAwait(false));

    /// <summary>Creates a scholarship programme.</summary>
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ScholarshipResponse>> Create(
        [FromBody] ScholarshipRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _scholarships.CreateAsync(request, cancellationToken).ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            return ToActionResult(result);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>Replaces a scholarship programme.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ScholarshipResponse>> Update(
        int id,
        [FromBody] ScholarshipRequest request,
        CancellationToken cancellationToken) =>
        ToActionResult(await _scholarships.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false));

    /// <summary>Deletes a scholarship programme and its applications.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _scholarships.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

        return result.IsSuccess ? NoContent() : ToActionResult(result).Result!;
    }
}
