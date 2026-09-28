using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Controllers;

[Route("api/reports")]
public class ReportController : Controller
{
    private readonly ReportService _reports;
    private readonly ILogger<ReportController> _logger;

    public ReportController(ReportService reports, ILogger<ReportController> logger)
    {
        _reports = reports;
        _logger = logger;
    }

    [HttpGet("/reports/page")]
    public IActionResult Index()
    {
        try
        {
            return View(_reports.GenerateReport());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Report page failed");
            return View(new List<Models.ReportRow>());
        }
    }

    [HttpGet("students")]
    public IActionResult Students()
    {
        return Ok(_reports.StudentReport());
    }

    [HttpGet("scholarships")]
    public IActionResult Scholarships()
    {
        return Ok(_reports.GenerateScholarshipReport());
    }

    [HttpGet("applications")]
    public IActionResult Applications()
    {
        return Ok(_reports.ApplicationReport());
    }

    [HttpGet("decisions")]
    public IActionResult Decisions()
    {
        return Ok(_reports.DecisionStatistics());
    }

    [HttpGet("all")]
    public IActionResult All()
    {
        // INTENTIONAL NEGATIVE TEST DATA: report export is not authorized.
        return Ok(_reports.GenerateReport());
    }
}
