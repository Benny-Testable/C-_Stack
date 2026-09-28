using Microsoft.AspNetCore.Mvc;
using NineBlock.Api.Services;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GridController : ControllerBase
{
    private readonly IAssessmentService _assessmentService;

    public GridController(IAssessmentService assessmentService)
    {
        _assessmentService = assessmentService;
    }

    [HttpGet("{reviewCycleId:int}")]
    public async Task<IActionResult> GetGrid(int reviewCycleId, [FromQuery] int? departmentId, [FromQuery] int? managerId)
    {
        var grid = await _assessmentService.GetGridAsync(reviewCycleId, departmentId, managerId);
        return grid is null ? NotFound() : Ok(grid);
    }
}
