using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.Models;
using NineBlock.Api.Services;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NineBoxGridController : ControllerBase
{
    private readonly NineBlockDbContext _context;
    private readonly NineBoxMatrixService _matrixService;

    public NineBoxGridController(NineBlockDbContext context, NineBoxMatrixService matrixService)
    {
        _context = context;
        _matrixService = matrixService;
    }

    [HttpGet("quadrants")]
    public async Task<ActionResult<IEnumerable<NineBoxQuadrant>>> GetQuadrants()
    {
        var quadrants = await _context.NineBoxQuadrants.OrderBy(q => q.BlockNumber).ToListAsync();
        return Ok(quadrants);
    }

    [HttpGet("distribution")]
    public async Task<ActionResult> GetGridDistribution([FromQuery] int? cycleId)
    {
        var query = _context.Assessments.Include(a => a.Employee).AsQueryable();
        if (cycleId.HasValue)
        {
            query = query.Where(a => a.ReviewCycleId == cycleId.Value);
        }

        var assessments = await query.ToListAsync();
        var grouped = assessments
            .GroupBy(a => a.AssignedBlockNumber)
            .Select(g => new
            {
                BlockNumber = g.Key,
                Count = g.Count(),
                Employees = g.Select(a => new
                {
                    a.EmployeeId,
                    EmployeeName = a.Employee?.FullName ?? "Unknown",
                    Department = a.Employee?.Department ?? "General",
                    a.PerformanceScore,
                    a.PotentialScore
                })
            });

        return Ok(grouped);
    }

    [HttpPost("calculate")]
    public IActionResult CalculateQuadrant([FromBody] ScoreCalculationRequest request)
    {
        try
        {
            var result = _matrixService.ResolveNineBoxQuadrant(request.PerformanceScore, request.PotentialScore);
            return Ok(new
            {
                BlockNumber = result.blockNumber,
                QuadrantName = result.quadrantName,
                ColorHex = result.colorHex
            });
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}

public record ScoreCalculationRequest(decimal PerformanceScore, decimal PotentialScore);
