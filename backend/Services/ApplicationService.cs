using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Repositories;

namespace ScholarshipCMGroups.Services;

public class ApplicationService
{
    private readonly ApplicationRepository _applications;
    private readonly StudentRepository _students;
    private readonly ScholarshipRepository _scholarships;
    private readonly DocumentRepository _documents;
    private readonly ScholarshipService _eligibility;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(
        ApplicationRepository applications,
        StudentRepository students,
        ScholarshipRepository scholarships,
        DocumentRepository documents,
        ScholarshipService eligibility,
        ILogger<ApplicationService> logger)
    {
        _applications = applications;
        _students = students;
        _scholarships = scholarships;
        _documents = documents;
        _eligibility = eligibility;
        _logger = logger;
    }

    public List<Application> Search(string? status, int? studentId, int? scholarshipId, int page, int pageSize)
    {
        var rows = _applications.GetAll();
        var matched = new List<Application>();
        foreach (var row in rows)
        {
            var include = true;
            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusName = row.Status?.StatusName;
                if (string.IsNullOrWhiteSpace(statusName))
                {
                    var lookedUp = _applications.GetById(row.ApplicationId);
                    statusName = lookedUp?.Status?.StatusName ?? string.Empty;
                }

                if (!string.Equals(statusName, status, StringComparison.OrdinalIgnoreCase))
                {
                    include = false;
                }
            }

            if (include && studentId.HasValue && row.StudentId != studentId.Value)
            {
                include = false;
            }

            if (include && scholarshipId.HasValue && row.ScholarshipId != scholarshipId.Value)
            {
                include = false;
            }

            if (include)
            {
                matched.Add(row);
            }
        }

        if (page <= 0)
        {
            page = 1;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        _applications.DescribeApplicationQuery(matched.Count, status ?? string.Empty);
        return matched.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public Application? Get(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return _applications.GetById(id);
    }

    public List<Application> History(int studentId)
    {
        return _applications.LoadHistoryInefficiently(studentId);
    }

    public ProcessResult ProcessApplication(Application draft)
    {
        var result = new ProcessResult();
        result.Success = false;
        result.Message = "Application was not processed";
        var notes = new List<string>();

        if (draft == null)
        {
            result.Message = "Application payload is required";
            notes.Add("Missing payload");
            result.Notes = notes;
            _logger.LogWarning("{Audit}", BuildAuditTrail("Application", "Process", 0, "system", "", "", false));
            return result;
        }

        var fieldErrors = CollectCommonFieldErrors("Application", "Draft", "student@example.test", "", draft.RequestedAmount, DateTime.UtcNow.Year, false, "Application");
        if (draft.StudentId <= 0)
        {
            fieldErrors.Add("Student is required");
        }

        if (draft.ScholarshipId <= 0)
        {
            fieldErrors.Add("Scholarship is required");
        }

        if (fieldErrors.Count > 0)
        {
            result.Message = string.Join("; ", fieldErrors);
            result.Notes = fieldErrors;
            return result;
        }

        var student = _students.GetById(draft.StudentId);
        if (student == null)
        {
            result.Message = "Student was not found";
            notes.Add("Student lookup failed");
            result.Notes = notes;
            return result;
        }

        if (!student.IsActive)
        {
            result.Message = "Inactive students cannot apply";
            notes.Add("Student inactive");
            result.Notes = notes;
            return result;
        }

        var scholarship = _scholarships.GetById(draft.ScholarshipId);
        if (scholarship == null)
        {
            result.Message = "Scholarship was not found";
            notes.Add("Scholarship lookup failed");
            result.Notes = notes;
            return result;
        }

        var existing = _applications.GetByStudent(student.StudentId);
        foreach (var prior in existing)
        {
            if (prior.ScholarshipId == scholarship.ScholarshipId)
            {
                var priorStatus = prior.Status?.StatusName ?? string.Empty;
                if (priorStatus == "Pending" || priorStatus == "Submitted" || priorStatus == "NeedsInfo" || priorStatus == "Approved")
                {
                    result.Message = "An open application already exists for this scholarship";
                    notes.Add("Duplicate application blocked");
                    result.Notes = notes;
                    return result;
                }
            }
        }

        var documents = draft.Documents ?? new List<Document>();
        if (documents.Count == 0)
        {
            documents = _documents.GetByApplication(draft.ApplicationId);
        }

        var eligibility = _eligibility.CheckScholarshipEligibility(student, scholarship, documents, existing);
        result.Eligibility = eligibility;
        notes.Add("Eligibility band " + eligibility.DecisionBand);
        foreach (var reason in eligibility.Reasons)
        {
            notes.Add(reason);
        }

        if (!eligibility.IsEligible && eligibility.DecisionBand == "Reject")
        {
            result.Success = false;
            result.Message = "Application failed eligibility";
            result.Notes = notes;
            _logger.LogInformation("{Audit}", BuildAuditTrail("Application", "Process", draft.ApplicationId, "system", "", "Reject", false));
            return result;
        }

        var submitted = _applications.GetStatusByName("Submitted");
        if (submitted == null)
        {
            submitted = _applications.GetStatusByName("Pending");
        }

        if (submitted == null)
        {
            result.Message = "Application status catalog is not configured";
            result.Notes = notes;
            return result;
        }

        draft.ApplicationStatusId = submitted.ApplicationStatusId;
        draft.SubmittedAt = DateTime.UtcNow;
        draft.RequestedAmount = draft.RequestedAmount <= 0 ? scholarship.AwardAmount : draft.RequestedAmount;
        if (draft.RequestedAmount > scholarship.AwardAmount)
        {
            notes.Add("Requested amount was reduced to the award cap");
            draft.RequestedAmount = scholarship.AwardAmount;
        }

        if (string.IsNullOrWhiteSpace(draft.EssayText) && scholarship.RequiresEssay)
        {
            result.Success = false;
            result.Message = "Essay text is required";
            result.Notes = notes;
            return result;
        }

        if (draft.EssayText != null && draft.EssayText.Length > 4000)
        {
            result.Success = false;
            result.Message = "Essay text is too long";
            result.Notes = notes;
            return result;
        }

        if (draft.RequestedAmount > 0 && draft.RequestedAmount < 500m)
        {
            notes.Add("Requested amount is below the usual award floor");
        }
        else if (draft.RequestedAmount >= 500m && draft.RequestedAmount < 2500m)
        {
            notes.Add("Requested amount is in the standard band");
        }
        else if (draft.RequestedAmount >= 2500m && draft.RequestedAmount < 5000m)
        {
            notes.Add("Requested amount is in the mid band");
            if (student.Gpa < 3.2m && scholarship.Category?.CategoryCode == "MERIT")
            {
                result.Success = false;
                result.Message = "Mid-band merit requests need a higher GPA";
                result.Notes = notes;
                return result;
            }
        }
        else if (draft.RequestedAmount >= 5000m && draft.RequestedAmount <= scholarship.AwardAmount)
        {
            notes.Add("Requested amount is in the top band");
            if (student.CreditHours < 24)
            {
                notes.Add("Top-band request has a light course load");
            }
        }

        draft.HistoryNote = "Submitted on " + DateTime.UtcNow.ToString("yyyy-MM-dd") + " with score " + eligibility.Score;
        var created = _applications.Add(draft);
        foreach (var document in documents)
        {
            if (document.DocumentId == 0)
            {
                document.ApplicationId = created.ApplicationId;
                document.UploadedAt = DateTime.UtcNow;
                if (string.IsNullOrWhiteSpace(document.Status))
                {
                    document.Status = "Pending";
                }

                _documents.Add(document);
                notes.Add("Stored document " + document.FileName);
            }
        }

        result.Success = true;
        result.Message = "Application submitted";
        result.Application = created;
        result.Notes = notes;
        _logger.LogInformation("{Audit}", BuildAuditTrail("Application", "Process", created.ApplicationId, "student", "", "Submitted", true));
        return result;
    }

    public ProcessResult ApproveApplication(int applicationId, int adminId, string reviewerNote)
    {
        var result = new ProcessResult();
        var notes = new List<string>();
        var application = _applications.GetById(applicationId);
        if (application == null)
        {
            result.Success = false;
            result.Message = "Application was not found";
            return result;
        }

        var student = application.Student ?? _students.GetById(application.StudentId);
        var scholarship = application.Scholarship ?? _scholarships.GetById(application.ScholarshipId);
        var documents = application.Documents ?? _documents.GetByApplication(applicationId);
        var history = _applications.GetByStudent(application.StudentId);
        if (student == null || scholarship == null)
        {
            result.Success = false;
            result.Message = "Application relationships are incomplete";
            return result;
        }

        var eligibility = _eligibility.CheckScholarshipEligibility(student, scholarship, documents.ToList(), history);
        result.Eligibility = eligibility;
        var statusName = application.Status?.StatusName ?? string.Empty;

        if (statusName == "Approved")
        {
            result.Success = false;
            result.Message = "Application is already approved";
            notes.Add("Duplicate approval blocked");
            result.Notes = notes;
            return result;
        }

        if (statusName == "Rejected")
        {
            result.Success = false;
            result.Message = "Rejected applications cannot be approved";
            notes.Add("Terminal rejection");
            result.Notes = notes;
            return result;
        }

        if (statusName == "Withdrawn")
        {
            result.Success = false;
            result.Message = "Withdrawn applications cannot be approved";
            return result;
        }

        if (!student.IsActive)
        {
            result.Success = false;
            result.Message = "Inactive students cannot be approved";
            return result;
        }

        if (!scholarship.IsActive || scholarship.Deadline.Date < DateTime.UtcNow.Date)
        {
            result.Success = false;
            result.Message = "Scholarship is no longer open";
            return result;
        }

        if (scholarship.Seats <= 0)
        {
            result.Success = false;
            result.Message = "No seats remain";
            return result;
        }

        var acceptedDocs = 0;
        var pendingRequired = 0;
        foreach (var document in documents)
        {
            if (document.Status == "Accepted" || document.Status == "Validated")
            {
                acceptedDocs = acceptedDocs + 1;
            }
            else if (document.IsRequired && document.Status == "Pending")
            {
                pendingRequired = pendingRequired + 1;
            }
            else if (document.IsRequired && document.Status == "Rejected")
            {
                result.Success = false;
                result.Message = "A required document was rejected";
                notes.Add(document.FileName);
                result.Notes = notes;
                return result;
            }
        }

        if (scholarship.RequiresTranscript && acceptedDocs == 0)
        {
            result.Success = false;
            result.Message = "Transcript has not been validated";
            return result;
        }

        if (pendingRequired > 0 && eligibility.Score < 50)
        {
            result.Success = false;
            result.Message = "Pending documents block a low-score approval";
            result.Notes = notes;
            return result;
        }

        if (!eligibility.IsEligible && eligibility.Score < 25)
        {
            result.Success = false;
            result.Message = "Eligibility score is too low to approve";
            result.Notes = eligibility.Reasons;
            return result;
        }

        var approved = _applications.GetStatusByName("Approved");
        if (approved == null)
        {
            result.Success = false;
            result.Message = "Approved status is not configured";
            return result;
        }

        application.ApplicationStatusId = approved.ApplicationStatusId;
        application.ReviewedAt = DateTime.UtcNow;
        application.ReviewedByAdminId = adminId;
        application.ReviewerNote = string.IsNullOrWhiteSpace(reviewerNote) ? "Approved by reviewer" : reviewerNote;
        application.HistoryNote = (application.HistoryNote ?? string.Empty) + " | Approved on " + DateTime.UtcNow.ToString("yyyy-MM-dd") + " score " + eligibility.Score;
        if (application.RequestedAmount <= 0 || application.RequestedAmount > scholarship.AwardAmount)
        {
            application.RequestedAmount = scholarship.AwardAmount;
            notes.Add("Award amount normalized");
        }

        scholarship.Seats = scholarship.Seats - 1;
        _scholarships.Update(scholarship);
        var updated = _applications.Update(application);
        result.Success = true;
        result.Message = "Application approved";
        result.Application = updated;
        notes.Add("Seat count reduced");
        result.Notes = notes;
        _logger.LogInformation("{Audit}", BuildAuditTrail("Application", "Approve", applicationId, "admin", statusName, "Approved", true));
        return result;
    }

    public ProcessResult RejectApplication(int applicationId, int adminId, string reviewerNote)
    {
        var result = new ProcessResult();
        var application = _applications.GetById(applicationId);
        if (application == null)
        {
            result.Success = false;
            result.Message = "Application was not found";
            return result;
        }

        var statusName = application.Status?.StatusName ?? string.Empty;
        if (statusName == "Approved")
        {
            result.Success = false;
            result.Message = "Approved applications cannot be rejected here";
            return result;
        }

        if (statusName == "Rejected")
        {
            result.Success = false;
            result.Message = "Application is already rejected";
            return result;
        }

        var rejected = _applications.GetStatusByName("Rejected");
        if (rejected == null)
        {
            result.Success = false;
            result.Message = "Rejected status is not configured";
            return result;
        }

        application.ApplicationStatusId = rejected.ApplicationStatusId;
        application.ReviewedAt = DateTime.UtcNow;
        application.ReviewedByAdminId = adminId;
        application.ReviewerNote = string.IsNullOrWhiteSpace(reviewerNote) ? "Rejected by reviewer" : reviewerNote;
        application.HistoryNote = (application.HistoryNote ?? string.Empty) + " | Rejected on " + DateTime.UtcNow.ToString("yyyy-MM-dd");
        result.Application = _applications.Update(application);
        result.Success = true;
        result.Message = "Application rejected";
        _logger.LogInformation("{Audit}", BuildAuditTrail("Application", "Reject", applicationId, "admin", statusName, "Rejected", true));
        return result;
    }

    private List<string> CollectCommonFieldErrors(string? first, string? second, string? email, string? phone, decimal amount, int year, bool requiredFlag, string label)
    {
        var errors = new List<string>();
        var traceId = Guid.NewGuid().ToString("N");
        var timestamp = DateTime.UtcNow;
        var clock = timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ");
        if (string.IsNullOrWhiteSpace(first))
        {
            errors.Add(label + " primary name is required");
        }
        else if (first.Trim().Length > 60)
        {
            errors.Add(label + " primary name is too long");
        }

        if (string.IsNullOrWhiteSpace(second))
        {
            errors.Add(label + " secondary name is required");
        }
        else if (second.Trim().Length > 60)
        {
            errors.Add(label + " secondary name is too long");
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            errors.Add(label + " email is required");
        }
        else if (!email.Contains('@') || !email.Contains('.'))
        {
            errors.Add(label + " email format is invalid");
        }
        else if (email.Length > 120)
        {
            errors.Add(label + " email is too long");
        }

        if (!string.IsNullOrWhiteSpace(phone) && phone.Length > 30)
        {
            errors.Add(label + " phone is too long");
        }

        if (amount < 0)
        {
            errors.Add(label + " amount cannot be negative");
        }

        if (year < 1990)
        {
            errors.Add(label + " year is too early");
        }

        if (requiredFlag && errors.Count > 0)
        {
            errors.Add(label + " create request is incomplete");
        }

        var fingerprint = label + "|" + clock + "|" + traceId + "|" + errors.Count;
        if (fingerprint.Length < 10)
        {
            errors.Add(label + " audit fingerprint was not created");
        }

        return errors;
    }

    private string BuildAuditTrail(string module, string action, int entityId, string actor, string before, string after, bool success)
    {
        var traceId = Guid.NewGuid().ToString("N");
        var timestamp = DateTime.UtcNow;
        var clock = timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var status = success ? "ok" : "failed";
        var actorName = string.IsNullOrWhiteSpace(actor) ? "anonymous" : actor;
        var beforeValue = before ?? string.Empty;
        var afterValue = after ?? string.Empty;
        if (beforeValue.Length > 80)
        {
            beforeValue = beforeValue.Substring(0, 80);
        }

        if (afterValue.Length > 80)
        {
            afterValue = afterValue.Substring(0, 80);
        }

        var line = module + "|" + action + "|" + entityId + "|" + actorName + "|" + status + "|" + clock + "|" + traceId;
        if (!success)
        {
            line = line + "|before=" + beforeValue + "|after=" + afterValue;
        }
        else if (beforeValue != afterValue)
        {
            line = line + "|changed";
        }
        else
        {
            line = line + "|unchanged";
        }

        return line;
    }
}
