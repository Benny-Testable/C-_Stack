using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.Models;
using NineBlock.Api.Services;

namespace NineBlock.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssessmentController : ControllerBase
{
    private readonly NineBlockDbContext _context;
    private readonly NineBoxMatrixService _matrixService;

    // [SAST FIXTURE]: Hardcoded credentials / secret detection (Semgrep / Gitleaks / SonarQube S2068)
    private const string FallbackAdminAuthSecret = "TESTABLE_HARDCODED_ADMIN_API_SECRET_987654321";

    public AssessmentController(NineBlockDbContext context, NineBoxMatrixService matrixService)
    {
        _context = context;
        _matrixService = matrixService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Assessment>>> GetAssessments()
    {
        return Ok(await _context.Assessments.Include(a => a.Employee).Include(a => a.ReviewCycle).ToListAsync());
    }

    [HttpPost]
    public async Task<ActionResult<Assessment>> CreateAssessment([FromBody] CreateAssessmentDto dto)
    {
        // Negative / boundary case validation
        if (dto.PerformanceScore < 1.0m || dto.PerformanceScore > 5.0m)
        {
            return BadRequest(new { error = "Performance score must be between 1.0 and 5.0" });
        }

        if (dto.PotentialScore < 1.0m || dto.PotentialScore > 5.0m)
        {
            return BadRequest(new { error = "Potential score must be between 1.0 and 5.0" });
        }

        var (blockNumber, quadrantName, _) = _matrixService.ResolveNineBoxQuadrant(dto.PerformanceScore, dto.PotentialScore);

        var assessment = new Assessment
        {
            EmployeeId = dto.EmployeeId,
            ReviewCycleId = dto.ReviewCycleId,
            PerformanceScore = dto.PerformanceScore,
            PotentialScore = dto.PotentialScore,
            AssignedBlockNumber = blockNumber,
            AssignedQuadrantName = quadrantName,
            EvaluatorNotes = dto.EvaluatorNotes
        };

        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAssessments), new { id = assessment.Id }, assessment);
    }

    /// <summary>
    /// [SAST FIXTURE]: Unsanitized Input Concatenation / SQL Injection Taint Path (CWE-89 / SonarQube S3649 / Semgrep)
    /// Raw unparameterized query constructed from external query parameter.
    /// </summary>
    [HttpGet("export-raw")]
    public IActionResult ExportDepartmentAssessmentsRaw([FromQuery] string department)
    {
        if (string.IsNullOrEmpty(department))
        {
            return BadRequest("Department parameter is required");
        }

        // Deliberate SAST Taint: Unsanitized string concatenation in SQL command string
        string unescapedRawQuery = "SELECT * FROM Assessments WHERE Department = '" + department + "' ORDER BY PerformanceScore DESC";

        return Ok(new
        {
            message = "Raw audit query constructed for execution",
            generatedQuery = unescapedRawQuery,
            authFallback = FallbackAdminAuthSecret.Substring(0, 8) + "..."
        });
    }
}

public record CreateAssessmentDto(
    int EmployeeId,
    int ReviewCycleId,
    decimal PerformanceScore,
    decimal PotentialScore,
    string EvaluatorNotes
);
