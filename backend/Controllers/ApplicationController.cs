using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Controllers;

[Route("api/applications")]
public class ApplicationController : Controller
{
    private readonly ApplicationService _applications;
    private readonly ILogger<ApplicationController> _logger;

    public ApplicationController(ApplicationService applications, ILogger<ApplicationController> logger)
    {
        _applications = applications;
        _logger = logger;
    }

    [HttpGet("/applications/page")]
    public IActionResult Index(string? status)
    {
        try
        {
            var rows = _applications.Search(status, null, null, 1, 50);
            return View(rows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application page failed");
            return View(new List<Application>());
        }
    }

    [HttpGet("")]
    public IActionResult List(string? status, int? studentId, int? scholarshipId, int page = 1, int pageSize = 10)
    {
        try
        {
            var rows = _applications.Search(status, studentId, scholarshipId, page, pageSize);
            return Ok(new { success = true, items = rows, page, pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application list failed");
            return StatusCode(500, BuildNegativeMetricResponse("Application", "List", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var row = _applications.Get(id);
        if (row == null)
        {
            return NotFound(BuildNegativeMetricResponse("Application", "Get", false, "Application was not found", 404, null, id, "anonymous"));
        }

        return Ok(row);
    }

    [HttpGet("history/{studentId:int}")]
    public IActionResult History(int studentId)
    {
        try
        {
            var rows = _applications.History(studentId);
            return Ok(new { success = true, items = rows });
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Application", "History", false, ex.Message, 500, ex, studentId, "anonymous"));
        }
    }

    [HttpPost("")]
    public IActionResult Apply([FromBody] Application draft)
    {
        try
        {
            var result = _applications.ProcessApplication(draft);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message, notes = result.Notes, eligibility = result.Eligibility });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application processing failed");
            return StatusCode(500, BuildNegativeMetricResponse("Application", "Apply", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpPost("{id:int}/approve")]
    public IActionResult Approve(int id, [FromBody] ReviewRequest? request)
    {
        // INTENTIONAL NEGATIVE TEST DATA: approval does not check an admin session.
        try
        {
            var adminId = request?.AdminId ?? 1;
            var note = request?.Note ?? "Approved";
            var result = _applications.ApproveApplication(id, adminId, note);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Approve failed");
            return StatusCode(500, BuildNegativeMetricResponse("Application", "Approve", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    [HttpPost("{id:int}/reject")]
    public IActionResult Reject(int id, [FromBody] ReviewRequest? request)
    {
        // INTENTIONAL NEGATIVE TEST DATA: rejection does not check an admin session.
        try
        {
            var adminId = request?.AdminId ?? 1;
            var note = request?.Note ?? "Rejected";
            var result = _applications.RejectApplication(id, adminId, note);
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Application", "Reject", false, ex.Message, 500, ex, id, "anonymous"));
        }
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

public class ReviewRequest
{
    public int AdminId { get; set; }
    public string Note { get; set; } = string.Empty;
}
