using Microsoft.AspNetCore.Mvc;
using NineBlock.Api.DTOs;
using NineBlock.Api.Services;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssessmentsController : ControllerBase
{
    private readonly IAssessmentService _assessmentService;

    public AssessmentsController(IAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssessmentRequest request)
    {
        if (request.PerformanceScore is < 1 or > 3 || request.PotentialScore is < 1 or > 3)
            return BadRequest("PerformanceScore and PotentialScore must each be between 1 and 3.");

        var result = await _assessmentService.CreateAsync(request);
        return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAssessmentRequest request)
    {
        if (request.PerformanceScore is < 1 or > 3 || request.PotentialScore is < 1 or > 3)
            return BadRequest("PerformanceScore and PotentialScore must each be between 1 and 3.");

        var result = await _assessmentService.UpdateAsync(id, request);
        return result is null ? NotFound() : Ok(result);
    }
}
