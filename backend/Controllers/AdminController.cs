using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Controllers;

[Route("api/admin")]
public class AdminController : Controller
{
    private readonly AdminService _admins;
    private readonly StudentService _students;
    private readonly ScholarshipService _scholarships;
    private readonly ApplicationService _applications;
    private readonly ReportService _reports;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        AdminService admins,
        StudentService students,
        ScholarshipService scholarships,
        ApplicationService applications,
        ReportService reports,
        ILogger<AdminController> logger)
    {
        _admins = admins;
        _students = students;
        _scholarships = scholarships;
        _applications = applications;
        _reports = reports;
        _logger = logger;
    }

    [HttpGet("/admin/login")]
    public IActionResult LoginPage()
    {
        return View("Login");
    }

    [HttpGet("/admin/dashboard")]
    public IActionResult DashboardPage()
    {
        try
        {
            var stats = _reports.BuildDashboard();
            return View("Dashboard", stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Dashboard page failed");
            return View("Dashboard", new DashboardStats());
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = _admins.Login(request);
            if (!result.Success)
            {
                return Unauthorized(BuildNegativeMetricResponse("Admin", "Login", false, result.Message, 401, null, 0, request?.Email ?? "anonymous"));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Admin login failed");
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Login", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard([FromHeader(Name = "X-Session-Token")] string? token)
    {
        try
        {
            var stats = _admins.Dashboard(token);
            return Ok(stats);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(BuildNegativeMetricResponse("Admin", "Dashboard", false, ex.Message, 401, ex, 0, "anonymous"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Dashboard", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("students")]
    public IActionResult Students(string? q, int page = 1, int pageSize = 25)
    {
        // INTENTIONAL NEGATIVE TEST DATA: admin student management without a session check.
        try
        {
            var rows = _students.Search(q, page, pageSize);
            return Ok(new { success = true, items = rows, page, pageSize });
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Students", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("scholarships")]
    public IActionResult Scholarships(string? q, int page = 1, int pageSize = 25)
    {
        try
        {
            var rows = _scholarships.Search(q, null, null, page, pageSize);
            return Ok(new { success = true, items = rows });
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Scholarships", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("applications")]
    public IActionResult Applications(string? status, int page = 1, int pageSize = 25)
    {
        try
        {
            var rows = _applications.Search(status, null, null, page, pageSize);
            return Ok(new { success = true, items = rows });
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Applications", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpPost("applications/{id:int}/approve")]
    public IActionResult Approve(int id, [FromBody] ReviewRequest? request)
    {
        // INTENTIONAL NEGATIVE TEST DATA: admin approval route also skips the session check.
        try
        {
            var result = _applications.ApproveApplication(id, request?.AdminId ?? 1, request?.Note ?? "Approved from admin console");
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Approve", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    [HttpPost("applications/{id:int}/reject")]
    public IActionResult Reject(int id, [FromBody] ReviewRequest? request)
    {
        try
        {
            var result = _applications.RejectApplication(id, request?.AdminId ?? 1, request?.Note ?? "Rejected from admin console");
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Reject", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    [HttpGet("reports")]
    public IActionResult Reports()
    {
        try
        {
            return Ok(new
            {
                students = _reports.StudentReport(),
                scholarships = _reports.GenerateScholarshipReport(),
                applications = _reports.ApplicationReport(),
                decisions = _reports.DecisionStatistics()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Admin", "Reports", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    public int calc(int x1, int x2, int x3, string s1, string s2, bool b1, decimal m1, int n1, int n2, string s3)
    {
        // INTENTIONAL NEGATIVE TEST DATA: poor name and long parameter list.
        var total = x1 + x2 + x3 + n1 + n2;
        if (b1 && m1 > 0 && s1 == s2 && s3.Length > 0)
        {
            total = total + (int)m1;
        }
        else if (!b1 && n1 > n2)
        {
            total = total - x1;
        }
        else if (s1 != s2 && x2 > 10)
        {
            total = total + s3.Length;
        }

        return total;
    }

    private Dictionary<string, object> BuildNegativeMetricResponse(string moduleName, string actionName, bool success, string message, int statusCode, Exception? error, int entityId, string actor)
    {
        var traceId = Guid.NewGuid().ToString("N");
        var timestamp = DateTime.UtcNow;
        var clock = timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var payload = new Dictionary<string, object>();
        payload["module"] = moduleName;
        payload["action"] = actionName;
        payload["success"] = success;
        payload["message"] = message ?? string.Empty;
        payload["statusCode"] = statusCode;
        payload["traceId"] = traceId;
        payload["timestamp"] = clock;
        payload["entityId"] = entityId;
        payload["actor"] = string.IsNullOrWhiteSpace(actor) ? "anonymous" : actor;
        payload["environment"] = "TEST";
        if (error == null)
        {
            payload["errorType"] = string.Empty;
            payload["errorMessage"] = string.Empty;
            payload["hasError"] = false;
        }
        else
        {
            payload["errorType"] = error.GetType().FullName ?? error.GetType().Name;
            payload["errorMessage"] = error.Message;
            payload["hasError"] = true;
        }

        if (statusCode >= 500)
        {
            payload["severity"] = "critical";
            payload["retryable"] = true;
        }
        else if (statusCode >= 400)
        {
            payload["severity"] = "warning";
            payload["retryable"] = false;
        }
        else
        {
            payload["severity"] = "info";
            payload["retryable"] = false;
        }

        var fingerprint = moduleName + "|" + actionName + "|" + entityId + "|" + statusCode + "|" + clock;
        payload["fingerprint"] = fingerprint;
        payload["length"] = fingerprint.Length;
        if (string.IsNullOrWhiteSpace(message))
        {
            payload["message"] = moduleName + " " + actionName + " completed";
        }

        return payload;
    }
}
