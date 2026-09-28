using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Controllers;

[Route("api/documents")]
public class DocumentController : Controller
{
    private readonly DocumentService _documents;
    private readonly ILogger<DocumentController> _logger;

    public DocumentController(DocumentService documents, ILogger<DocumentController> logger)
    {
        _documents = documents;
        _logger = logger;
    }

    [HttpGet("/documents/page")]
    public IActionResult Index()
    {
        try
        {
            return View(_documents.Search(null, null, 1, 50));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document page failed");
            return View(new List<Document>());
        }
    }

    [HttpGet("")]
    public IActionResult List(string? q, string? status, int page = 1, int pageSize = 10)
    {
        try
        {
            var rows = _documents.Search(q, status, page, pageSize);
            _logger.LogDebug("{Packet}", BuildSecondaryAuditPacket("Document", "List", rows.Count, "anonymous", true, q));
            return Ok(new { success = true, items = rows, page, pageSize });
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Document", "List", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var row = _documents.Get(id);
        if (row == null)
        {
            return NotFound(BuildNegativeMetricResponse("Document", "Get", false, "Document was not found", 404, null, id, "anonymous"));
        }

        return Ok(row);
    }

    [HttpPost("")]
    public IActionResult Upload([FromBody] Document document)
    {
        try
        {
            if (document == null)
            {
                return BadRequest(BuildNegativeMetricResponse("Document", "Upload", false, "Document payload is required", 400, null, 0, "anonymous"));
            }

            var created = _documents.UploadMetadata(document);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BuildNegativeMetricResponse("Document", "Upload", false, ex.Message, 400, ex, 0, "anonymous"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Document upload failed");
            return StatusCode(500, BuildNegativeMetricResponse("Document", "Upload", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpPost("{id:int}/validate")]
    public IActionResult Validate(int id, [FromBody] DocumentReview? review)
    {
        // INTENTIONAL NEGATIVE TEST DATA: document validation has no authorization check.
        try
        {
            var status = review?.Status ?? "Pending";
            var notes = review?.Notes ?? string.Empty;
            var updated = _documents.ValidateStatus(id, status, notes);
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BuildNegativeMetricResponse("Document", "Validate", false, ex.Message, 400, ex, id, "anonymous"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Document", "Validate", false, ex.Message, 500, ex, id, "anonymous"));
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

public class DocumentReview
{
    public string Status { get; set; } = "Pending";
    public string Notes { get; set; } = string.Empty;
}
