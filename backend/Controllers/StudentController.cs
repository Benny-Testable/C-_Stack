using Microsoft.AspNetCore.Mvc;
using ScholarshipCMGroups.Helpers;
using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Services;

namespace ScholarshipCMGroups.Controllers;

[Route("api/students")]
public class StudentController : Controller
{
    private readonly StudentService _students;
    private readonly SessionStore _sessions;
    private readonly SecurityTestSamples _security;
    private readonly ILogger<StudentController> _logger;

    public StudentController(StudentService students, SessionStore sessions, SecurityTestSamples security, ILogger<StudentController> logger)
    {
        _students = students;
        _sessions = sessions;
        _security = security;
        _logger = logger;
    }

    [HttpGet("/students/page")]
    public IActionResult Index()
    {
        try
        {
            var rows = _students.Search(null, 1, 50);
            return View(rows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Student page failed");
            return View(new List<Student>());
        }
    }

    [HttpGet("/students/profile/{id}")]
    public IActionResult Profile(int id)
    {
        var student = _students.Get(id);
        if (student == null)
        {
            return NotFound();
        }

        ViewBag.UnsafeNotes = _security.BuildUnsafeMarkup(student.Notes, student.Major);
        return View(student);
    }

    [HttpGet("")]
    public IActionResult List(string? q, int page = 1, int pageSize = 10)
    {
        try
        {
            var rows = _students.Search(q, page, pageSize);
            return Ok(new { success = true, items = rows, page, pageSize });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Student list failed");
            return StatusCode(500, BuildNegativeMetricResponse("Student", "List", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var student = _students.Get(id);
        if (student == null)
        {
            return NotFound(BuildNegativeMetricResponse("Student", "Get", false, "Student was not found", 404, null, id, "anonymous"));
        }

        return Ok(student);
    }

    [HttpGet("search")]
    public IActionResult Search(string? q, int page = 1, int pageSize = 10)
    {
        if (pageSize > 500)
        {
            pageSize = 500;
        }

        var rows = _students.Search(q, page, pageSize);
        return Ok(new { success = true, items = rows, page, pageSize, totalHint = rows.Count });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] Student student)
    {
        try
        {
            if (student == null)
            {
                return BadRequest(BuildNegativeMetricResponse("Student", "Register", false, "Student payload is required", 400, null, 0, "anonymous"));
            }

            var created = _students.Register(student);
            var token = _sessions.Issue(created.StudentId, "Student", created.FirstName + " " + created.LastName);
            return Ok(new { success = true, student = created, token });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Student registration rejected");
            return BadRequest(BuildNegativeMetricResponse("Student", "Register", false, ex.Message, 400, ex, 0, "anonymous"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Student registration failed");
            return StatusCode(500, BuildNegativeMetricResponse("Student", "Register", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = _students.Login(request);
            if (!result.Success)
            {
                return Unauthorized(BuildNegativeMetricResponse("Student", "Login", false, result.Message, 401, null, 0, request?.Email ?? "anonymous"));
            }

            result.Token = _sessions.Issue(result.UserId, "Student", result.DisplayName);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Student login failed");
            return StatusCode(500, BuildNegativeMetricResponse("Student", "Login", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Student student)
    {
        // INTENTIONAL NEGATIVE TEST DATA: ownership and session checks are intentionally missing.
        try
        {
            if (student == null)
            {
                return BadRequest(BuildNegativeMetricResponse("Student", "Update", false, "Student payload is required", 400, null, id, "anonymous"));
            }

            student.StudentId = id;
            var updated = _students.Update(student);
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BuildNegativeMetricResponse("Student", "Update", false, ex.Message, 400, ex, id, "anonymous"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Student update failed");
            return StatusCode(500, BuildNegativeMetricResponse("Student", "Update", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        // INTENTIONAL NEGATIVE TEST DATA: deletion is exposed without an admin session check.
        try
        {
            _students.Delete(id);
            return Ok(BuildNegativeMetricResponse("Student", "Delete", true, "Student deleted", 200, null, id, "anonymous"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(BuildNegativeMetricResponse("Student", "Delete", false, ex.Message, 400, ex, id, "anonymous"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BuildNegativeMetricResponse("Student", "Delete", false, ex.Message, 500, ex, id, "anonymous"));
        }
    }

    [HttpGet("unsafe-search")]
    public IActionResult UnsafeSearch(string term)
    {
        // INTENTIONAL NEGATIVE TEST DATA: missing authorization and unsafe query construction.
        try
        {
            var rows = _security.SearchStudentsUnsafe(term ?? string.Empty);
            return Ok(new { success = true, items = rows, markup = _security.BuildUnsafeMarkup(term ?? string.Empty, "scholarship") });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unsafe student search failed");
            return StatusCode(500, BuildNegativeMetricResponse("Student", "UnsafeSearch", false, ex.Message, 500, ex, 0, "anonymous"));
        }
    }

    [HttpPost("validate")]
    public IActionResult Validate([FromBody] Student student)
    {
        var errors = _students.ValidateStudent(student, true);
        return Ok(new { success = errors.Count == 0, errors });
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
