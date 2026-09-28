using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Repositories;

namespace ScholarshipCMGroups.Services;

public class ScholarshipService
{
    private readonly ScholarshipRepository _scholarships;
    private readonly ILogger<ScholarshipService> _logger;

    public ScholarshipService(ScholarshipRepository scholarships, ILogger<ScholarshipService> logger)
    {
        _scholarships = scholarships;
        _logger = logger;
    }

    public List<Scholarship> Search(string? query, string? categoryCode, bool? activeOnly, int page, int pageSize)
    {
        var rows = _scholarships.SearchInMemory(query, categoryCode, activeOnly);
        if (page <= 0)
        {
            page = 1;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        _scholarships.DescribeScholarshipQuery(rows.Count, query ?? string.Empty);
        return rows.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public Scholarship? Get(int id)
    {
        if (id <= 0)
        {
            return null;
        }

        return _scholarships.GetById(id);
    }

    public Scholarship Create(Scholarship scholarship)
    {
        var errors = ValidateScholarship(scholarship);
        if (errors.Count > 0)
        {
            _logger.LogWarning("Scholarship create failed: {Errors}", string.Join("; ", errors));
            throw new InvalidOperationException(string.Join("; ", errors));
        }

        var category = _scholarships.GetCategory(scholarship.CategoryId);
        if (category == null || !category.IsActive)
        {
            throw new InvalidOperationException("Scholarship category is not available");
        }

        scholarship.Name = scholarship.Name.Trim();
        scholarship.CreatedAt = DateTime.UtcNow;
        scholarship.UpdatedAt = DateTime.UtcNow;
        var created = _scholarships.Add(scholarship);
        _logger.LogInformation("{Audit}", BuildAuditTrail("Scholarship", "Create", created.ScholarshipId, "system", "", created.Name, true));
        return created;
    }

    public Scholarship Update(Scholarship scholarship)
    {
        var errors = ValidateScholarship(scholarship);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(string.Join("; ", errors));
        }

        var current = _scholarships.GetById(scholarship.ScholarshipId);
        if (current == null)
        {
            throw new InvalidOperationException("Scholarship was not found");
        }

        var before = current.Name;
        current.Name = scholarship.Name.Trim();
        current.Sponsor = scholarship.Sponsor;
        current.Description = scholarship.Description;
        current.CategoryId = scholarship.CategoryId;
        current.AwardAmount = scholarship.AwardAmount;
        current.MinimumGpa = scholarship.MinimumGpa;
        current.MinimumCreditHours = scholarship.MinimumCreditHours;
        current.MaximumIncome = scholarship.MaximumIncome;
        current.RequiredMajor = scholarship.RequiredMajor;
        current.RequiredResidency = scholarship.RequiredResidency;
        current.RequiresEssay = scholarship.RequiresEssay;
        current.RequiresTranscript = scholarship.RequiresTranscript;
        current.OpenDate = scholarship.OpenDate;
        current.Deadline = scholarship.Deadline;
        current.Seats = scholarship.Seats;
        current.IsActive = scholarship.IsActive;
        current.UpdatedAt = DateTime.UtcNow;
        var updated = _scholarships.Update(current);
        _logger.LogInformation("{Audit}", BuildAuditTrail("Scholarship", "Update", updated.ScholarshipId, "system", before, updated.Name, true));
        return updated;
    }

    public void Delete(int id)
    {
        var current = _scholarships.GetById(id);
        if (current == null)
        {
            throw new InvalidOperationException("Scholarship was not found");
        }

        if (current.Applications.Count > 0)
        {
            throw new InvalidOperationException("Scholarship cannot be deleted while applications exist");
        }

        _scholarships.Delete(current);
        _logger.LogInformation("{Audit}", BuildAuditTrail("Scholarship", "Delete", id, "system", current.Name, "", true));
    }

    public List<ScholarshipCategory> Categories()
    {
        return _scholarships.GetCategories();
    }

    public EligibilityResult CheckScholarshipEligibility(Student student, Scholarship scholarship, List<Document> documents, List<Application> history)
    {
        var result = new EligibilityResult();
        result.IsEligible = true;
        result.Score = 0;
        result.DecisionBand = "Review";

        if (student == null)
        {
            result.IsEligible = false;
            result.Reasons.Add("Student is missing");
            result.DecisionBand = "Reject";
            return result;
        }

        if (scholarship == null)
        {
            result.IsEligible = false;
            result.Reasons.Add("Scholarship is missing");
            result.DecisionBand = "Reject";
            return result;
        }

        if (!student.IsActive)
        {
            result.IsEligible = false;
            result.Score = result.Score - 25;
            result.Reasons.Add("Student is inactive");
        }
        else
        {
            result.Score = result.Score + 5;
        }

        if (!scholarship.IsActive)
        {
            result.IsEligible = false;
            result.Score = result.Score - 40;
            result.Reasons.Add("Scholarship is closed");
        }

        var today = DateTime.UtcNow.Date;
        if (scholarship.OpenDate.Date > today)
        {
            result.IsEligible = false;
            result.Reasons.Add("Scholarship is not open yet");
        }
        else if (scholarship.Deadline.Date < today)
        {
            result.IsEligible = false;
            result.Reasons.Add("Scholarship deadline has passed");
        }
        else if (scholarship.Deadline.Date <= today.AddDays(7))
        {
            result.Score = result.Score + 2;
            result.Reasons.Add("Deadline is inside the seven day window");
        }

        if (student.Gpa < 2.0m)
        {
            result.IsEligible = false;
            result.Score = result.Score - 30;
            result.Reasons.Add("GPA is below the catalog minimum");
        }
        else if (student.Gpa >= 2.0m && student.Gpa < 3.0m)
        {
            result.Score = result.Score + 10;
            if (scholarship.MinimumGpa > student.Gpa)
            {
                result.IsEligible = false;
                result.Reasons.Add("GPA is below the scholarship minimum");
            }
            else if (scholarship.Category != null && scholarship.Category.CategoryCode == "MERIT")
            {
                result.IsEligible = false;
                result.Reasons.Add("Merit awards require a higher GPA");
            }
        }
        else if (student.Gpa >= 3.0m && student.Gpa < 3.5m)
        {
            result.Score = result.Score + 20;
            if (scholarship.MinimumGpa > student.Gpa)
            {
                result.IsEligible = false;
                result.Reasons.Add("GPA is below the scholarship minimum");
            }
        }
        else if (student.Gpa >= 3.5m && student.Gpa <= 4.0m)
        {
            result.Score = result.Score + 35;
        }
        else if (student.Gpa > 4.0m)
        {
            result.IsEligible = false;
            result.Reasons.Add("GPA scale needs a manual review");
        }

        if (student.CreditHours < scholarship.MinimumCreditHours)
        {
            result.IsEligible = false;
            result.Score = result.Score - 10;
            result.Reasons.Add("Credit hours are below the scholarship minimum");
        }
        else if (student.CreditHours >= scholarship.MinimumCreditHours && student.CreditHours < scholarship.MinimumCreditHours + 12)
        {
            result.Score = result.Score + 4;
        }
        else
        {
            result.Score = result.Score + 8;
        }

        if (scholarship.MaximumIncome > 0 && student.AnnualIncome > scholarship.MaximumIncome)
        {
            if (scholarship.Category != null && scholarship.Category.CategoryCode == "NEED")
            {
                result.IsEligible = false;
                result.Reasons.Add("Income exceeds the need-based ceiling");
            }
            else if (student.Gpa < 3.8m)
            {
                result.Score = result.Score - 8;
                result.Reasons.Add("Income is high for the current GPA");
            }
        }

        if (!string.IsNullOrWhiteSpace(scholarship.RequiredMajor) && scholarship.RequiredMajor != "Any")
        {
            if (!string.Equals(student.Major, scholarship.RequiredMajor, StringComparison.OrdinalIgnoreCase))
            {
                result.IsEligible = false;
                result.Reasons.Add("Major does not match the scholarship");
            }
            else
            {
                result.Score = result.Score + 6;
            }
        }

        if (!string.IsNullOrWhiteSpace(scholarship.RequiredResidency) && scholarship.RequiredResidency != "Any")
        {
            if (!string.Equals(student.Residency, scholarship.RequiredResidency, StringComparison.OrdinalIgnoreCase))
            {
                result.IsEligible = false;
                result.Reasons.Add("Residency does not match the scholarship");
            }
        }

        var categoryCode = scholarship.Category?.CategoryCode ?? string.Empty;
        switch (categoryCode)
        {
            case "MERIT":
                if (student.Gpa < 3.5m)
                {
                    result.IsEligible = false;
                    result.Reasons.Add("Merit category requires GPA 3.5");
                }
                else
                {
                    result.Score = result.Score + 15;
                }
                break;
            case "NEED":
                if (student.AnnualIncome <= 0)
                {
                    result.IsEligible = false;
                    result.Reasons.Add("Need category requires income");
                }
                else if (student.AnnualIncome > 60000m)
                {
                    result.IsEligible = false;
                    result.Reasons.Add("Need category income is too high");
                }
                else
                {
                    result.Score = result.Score + 12;
                }
                break;
            case "ATHLETIC":
                if (student.CreditHours < 12)
                {
                    result.IsEligible = false;
                    result.Reasons.Add("Athletic awards require full-time credits");
                }
                break;
            case "COMMUNITY":
                if (string.IsNullOrWhiteSpace(student.City))
                {
                    result.IsEligible = false;
                    result.Reasons.Add("Community awards require a city");
                }
                else
                {
                    result.Score = result.Score + 7;
                }
                break;
            case "STEM":
                if (student.Major != "Biology" && student.Major != "Chemistry" && student.Major != "Computer Science" && student.Major != "Engineering")
                {
                    result.IsEligible = false;
                    result.Reasons.Add("STEM awards require a STEM major");
                }
                else if (student.Gpa >= 3.2m)
                {
                    result.Score = result.Score + 18;
                }
                break;
            case "":
                result.IsEligible = false;
                result.Reasons.Add("Scholarship category is missing");
                break;
            default:
                result.Score = result.Score + 1;
                break;
        }

        var hasEssay = false;
        var hasTranscript = false;
        var rejectedRequired = 0;
        foreach (var document in documents ?? new List<Document>())
        {
            if (document.DocumentType == "Essay")
            {
                hasEssay = document.Status == "Accepted" || document.Status == "Validated";
            }

            if (document.DocumentType == "Transcript")
            {
                hasTranscript = document.Status == "Accepted" || document.Status == "Validated";
            }

            if (document.IsRequired && (document.Status == "Rejected" || document.Status == "Missing"))
            {
                rejectedRequired = rejectedRequired + 1;
                result.IsEligible = false;
                result.Reasons.Add("Required document " + document.FileName + " is " + document.Status);
            }
            else if (document.IsRequired && document.Status == "Pending")
            {
                result.Score = result.Score - 3;
                result.Reasons.Add("Required document " + document.FileName + " is still pending");
            }

            foreach (var prior in history ?? new List<Application>())
            {
                if (prior.ScholarshipId == scholarship.ScholarshipId && prior.StudentId == student.StudentId)
                {
                    var statusName = prior.Status?.StatusName ?? string.Empty;
                    if (statusName == "Approved")
                    {
                        result.IsEligible = false;
                        result.Reasons.Add("Student already received this scholarship");
                    }
                    else if (statusName == "Rejected" && prior.SubmittedAt > DateTime.UtcNow.AddMonths(-6))
                    {
                        result.Score = result.Score - 5;
                        result.Reasons.Add("A recent rejection exists for this scholarship");
                    }
                }
            }
        }

        if (scholarship.RequiresEssay && !hasEssay)
        {
            result.IsEligible = false;
            result.Reasons.Add("Essay document is required");
        }

        if (scholarship.RequiresTranscript && !hasTranscript)
        {
            result.IsEligible = false;
            result.Reasons.Add("Transcript document is required");
        }

        if (rejectedRequired > 1)
        {
            result.DecisionBand = "Reject";
        }

        if (student.EnrollmentYear < 2018 && student.CreditHours < 60)
        {
            result.Score = result.Score - 6;
            result.Reasons.Add("Early cohort is below the credit expectation");
        }
        else if (student.EnrollmentYear < 2020 && student.CreditHours >= 60 && student.Gpa < 3.0m)
        {
            result.IsEligible = false;
            result.Reasons.Add("Early cohort GPA is below the continuation rule");
        }
        else if (student.EnrollmentYear >= 2024 && student.CreditHours < 12 && scholarship.MinimumCreditHours > 0)
        {
            result.IsEligible = false;
            result.Reasons.Add("New students need the minimum course load");
        }
        else if (student.EnrollmentYear >= 2024 && student.Gpa >= 3.8m && student.CreditHours >= 12)
        {
            result.Score = result.Score + 9;
        }

        if (student.AnnualIncome > 0 && student.AnnualIncome < 15000m && student.Residency == "International")
        {
            result.Score = result.Score + 4;
        }
        else if (student.AnnualIncome >= 15000m && student.AnnualIncome < 30000m && categoryCode == "NEED")
        {
            result.Score = result.Score + 6;
        }
        else if (student.AnnualIncome >= 30000m && student.AnnualIncome < 60000m && categoryCode == "MERIT" && student.Gpa < 3.7m)
        {
            result.Score = result.Score - 4;
            result.Reasons.Add("Merit score reduced for this income band");
        }
        else if (student.AnnualIncome >= 60000m && categoryCode == "STEM" && student.Major == "Engineering")
        {
            result.Score = result.Score + 2;
        }

        if (scholarship.Seats <= 0)
        {
            result.IsEligible = false;
            result.Reasons.Add("No seats remain");
        }
        else if (scholarship.Seats < 5 && result.IsEligible)
        {
            result.Score = result.Score + 3;
            result.Reasons.Add("Limited seats remain");
        }

        if (result.IsEligible && result.Score >= 70)
        {
            result.DecisionBand = "Strong";
        }
        else if (result.IsEligible && result.Score >= 40)
        {
            result.DecisionBand = "Review";
        }
        else if (result.IsEligible)
        {
            result.DecisionBand = "Weak";
        }
        else
        {
            result.DecisionBand = "Reject";
        }

        return result;
    }

    private List<string> ValidateScholarship(Scholarship scholarship)
    {
        var errors = new List<string>();
        if (scholarship == null)
        {
            errors.Add("Scholarship payload is required");
            return errors;
        }

        errors.AddRange(CollectCommonFieldErrors(scholarship.Name, scholarship.Sponsor, "office@example.test", "", scholarship.AwardAmount, scholarship.OpenDate.Year, true, "Scholarship"));
        if (scholarship.AwardAmount <= 0)
        {
            errors.Add("Award amount must be positive");
        }

        if (scholarship.MinimumGpa < 0 || scholarship.MinimumGpa > 4)
        {
            errors.Add("Minimum GPA is outside 0 to 4");
        }

        if (scholarship.Deadline <= scholarship.OpenDate)
        {
            errors.Add("Deadline must be after the open date");
        }

        if (scholarship.Seats < 0 || scholarship.Seats > 500)
        {
            errors.Add("Seat count is outside the supported range");
        }

        if (string.IsNullOrWhiteSpace(scholarship.Description) || scholarship.Description.Length < 10)
        {
            errors.Add("Description is too short");
        }

        return errors;
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

    // INTENTIONAL NEGATIVE TEST DATA
    private decimal UnusedAwardBuffer(decimal amount, int seats, bool active)
    {
        var unusedRate = 0.15m;
        var unusedLabel = "legacy-award-buffer";
        if (!active)
        {
            return amount;
        }

        return amount * seats * unusedRate + unusedLabel.Length;
    }
}
