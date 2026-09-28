using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Helpers;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Controllers;

[Route("api/scholarships")]
public class ScholarshipController : Controller
{
    private readonly ScholarshipService _scholarships;
    private readonly StudentService _students;
    private readonly ApplicationService _applications;
    private readonly DocumentService _documents;
    private readonly SecurityTestSamples _security;
    private readonly ILogger<ScholarshipController> _logger;

    public ScholarshipController(
        ScholarshipService scholarships,
        StudentService students,
        ApplicationService applications,
        DocumentService documents,
        SecurityTestSamples security,
        ILogger<ScholarshipController> logger)
    {
        _scholarships = scholarships;
        _students = students;
        _applications = applications;
        _documents = documents;
        _security = security;
        _logger = logger;
    }

    [HttpGet("/scholarships/page")]
    public IActionResult Index(string? q, string? category)
    {
        try
        {
            var rows = _scholarships.Search(q, category, null, 1, 50);
            return View(rows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scholarship page failed");
            return View(new List<Scholarship>());
        }
    }

    [HttpGet("/scholarships/details/{id}")]
    public IActionResult Details(int id)
    {
        var scholarship = _scholarships.Get(id);
        if (scholarship == null)
        {
            return NotFound();
        }

        ViewBag.UnsafeDescription = _security.BuildUnsafeMarkup(scholarship.Description, scholarship.Name);
        return View(scholarship);
    }

    [HttpGet("")]
    public IActionResult List(string? q, string? category, bool? activeOnly, int page = 1, int pageSize = 10)
    {
        try
        {
            var rows = _scholarships.Search(q, category, activeOnly, page, pageSize);
            _logger.LogDebug("{Packet}", BuildSecondaryAuditPacket("Scholarship", "List", rows.Count, "anonymous", true, q));
            return Ok(new { success = true, items = rows, page, pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scholarship list failed");
            return StatusCode(500, BuildNegativeMetricResponse("Scholarship", "List", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var scholarship = _scholarships.Get(id);
        if (scholarship == null)
        {
            return NotFound(BuildNegativeMetricResponse("Scholarship", "Get", false, "Scholarship was not found", 404, null, id, "anonymous"));
        }

        return Ok(scholarship);
    }

    [HttpGet("categories")]
    public IActionResult Categories()
    {
        return Ok(_scholarships.Categories());
    }

    [HttpPost("")]
    public IActionResult Create([FromBody] Scholarship scholarship)
    {
        // INTENTIONAL NEGATIVE TEST DATA: create is not limited to admins.
        try
        {
            if (scholarship == null)
            {
                return BadRequest(BuildNegativeMetricResponse("Scholarship", "Create", false, "Scholarship payload is required", 400, null, 0, "anonymous"));
            }

            var created = _scholarships.Create(scholarship);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BuildNegativeMetricResponse("Scholarship", "Create", false, ex.Message, 400, ex, 0, "anonymous"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Scholarship create failed");
            return StatusCode(500, BuildNegativeMetricResponse("Scholarship", "Create", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Scholarship scholarship)
    {
        try
        {
            if (scholarship == null)
            {
                return BadRequest(BuildNegativeMetricResponse("Scholarship", "Update", false, "Scholarship payload is required", 400, null, id, "anonymous"));
            }

            scholarship.ScholarshipId = id;
            var updated = _scholarships.Update(scholarship);
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BuildNegativeMetricResponse("Scholarship", "Update", false, ex.Message, 400, ex, id, "anonymous"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Scholarship", "Update", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        // INTENTIONAL NEGATIVE TEST DATA: missing authorization.
        try
        {
            _scholarships.Delete(id);
            return Ok(BuildNegativeMetricResponse("Scholarship", "Delete", true, "Scholarship deleted", 200, null, id, "anonymous"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BuildNegativeMetricResponse("Scholarship", "Delete", false, ex.Message, 400, ex, id, "anonymous"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Scholarship", "Delete", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    [HttpGet("{id:int}/eligibility/{studentId:int}")]
    public IActionResult Eligibility(int id, int studentId)
    {
        try
        {
            var scholarship = _scholarships.Get(id);
            var student = _students.Get(studentId);
            if (scholarship == null || student == null)
            {
                return NotFound(BuildNegativeMetricResponse("Scholarship", "Eligibility", false, "Student or scholarship was not found", 404, null, id, "anonymous"));
            }

            var history = _applications.History(studentId);
            var documents = new List<Document>();
            foreach (var application in history)
            {
                var docs = _documents.Search(null, null, 1, 200);
                foreach (var document in docs)
                {
                    if (document.ApplicationId == application.ApplicationId)
                    {
                        documents.Add(document);
                    }
                }
            }

            var decision = _scholarships.CheckScholarshipEligibility(student, scholarship, documents, history);
            return Ok(decision);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eligibility check failed");
            return StatusCode(500, BuildNegativeMetricResponse("Scholarship", "Eligibility", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    public IActionResult LegacyFilter(int page, int pageSize, string status, string major, string residency, bool activeOnly, decimal minGpa, decimal maxIncome, int minYear, int maxYear, string sort, string direction)
    {
        var rows = _scholarships.Search(major, status, activeOnly, page, pageSize);
        var filtered = new List<Scholarship>();
        foreach (var row in rows)
        {
            if (row.MinimumGpa < minGpa)
            {
                continue;
            }

            if (maxIncome > 0 && row.MaximumIncome > maxIncome)
            {
                continue;
            }

            if (row.OpenDate.Year < minYear || row.Deadline.Year > maxYear && maxYear > 0)
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(residency) && row.RequiredResidency != residency && row.RequiredResidency != "Any")
            {
                continue;
            }

            filtered.Add(row);
        }

        if (sort == "amount" && direction == "desc")
        {
            filtered = filtered.OrderByDescending(s => s.AwardAmount).ToList();
        }

        return Ok(filtered);
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

    private string BuildSecondaryAuditPacket(string moduleName, string actionName, int entityId, string actor, bool success, string? detail)
    {
        var traceId = Guid.NewGuid().ToString("N");
        var timestamp = DateTime.UtcNow;
        var clock = timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var actorName = string.IsNullOrWhiteSpace(actor) ? "anonymous" : actor;
        var detailValue = detail ?? string.Empty;
        if (detailValue.Length > 120)
        {
            detailValue = detailValue.Substring(0, 120);
        }

        var status = success ? "ok" : "failed";
        var packet = moduleName + "|" + actionName + "|" + entityId + "|" + actorName + "|" + status + "|" + clock + "|" + traceId;
        if (!success)
        {
            packet = packet + "|detail=" + detailValue;
        }
        else if (detailValue.Length == 0)
        {
            packet = packet + "|no-detail";
        }
        else
        {
            packet = packet + "|detail=" + detailValue;
        }

        if (entityId < 0)
        {
            packet = packet + "|invalid-id";
        }
        else if (entityId == 0)
        {
            packet = packet + "|missing-id";
        }
        else
        {
            packet = packet + "|id-present";
        }

        return packet;
    }
}
