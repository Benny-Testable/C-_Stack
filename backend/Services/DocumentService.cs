using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Repositories;

namespace ScholarshipCMGroups.Services;

public class DocumentService
{
    private readonly DocumentRepository _documents;
    private readonly ApplicationRepository _applications;
    private readonly ILogger<DocumentService> _logger;

    public DocumentService(DocumentRepository documents, ApplicationRepository applications, ILogger<DocumentService> logger)
    {
        _documents = documents;
        _applications = applications;
        _logger = logger;
    }

    public List<Document> Search(string? query, string? status, int page, int pageSize)
    {
        var rows = _documents.GetAll();
        var matched = new List<Document>();
        foreach (var row in rows)
        {
            var include = true;
            if (!string.IsNullOrWhiteSpace(status) && !string.Equals(row.Status, status, StringComparison.OrdinalIgnoreCase))
            {
                include = false;
            }

            if (include && !string.IsNullOrWhiteSpace(query))
            {
                var blob = (row.FileName + " " + row.DocumentType + " " + row.Notes).ToLowerInvariant();
                if (!blob.Contains(query.Trim().ToLowerInvariant()))
                {
                    include = false;
                }
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

        _documents.DescribeDocumentQuery(matched.Count, query ?? string.Empty);
        return matched.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    }

    public Document? Get(int id)
    {
        return id <= 0 ? null : _documents.GetById(id);
    }

    public Document UploadMetadata(Document document)
    {
        var errors = ValidateDocument(document);
        if (errors.Count > 0)
        {
            _logger.LogWarning("Document metadata failed validation: {Errors}", string.Join("; ", errors));
            throw new InvalidOperationException(string.Join("; ", errors));
        }

        var application = _applications.GetById(document.ApplicationId);
        if (application == null)
        {
            throw new InvalidOperationException("Application was not found");
        }

        document.UploadedAt = DateTime.UtcNow;
        if (string.IsNullOrWhiteSpace(document.Status))
        {
            document.Status = "Pending";
        }

        var created = _documents.Add(document);
        _logger.LogInformation("{Audit}", BuildAuditTrail("Document", "Upload", created.DocumentId, "student", "", created.FileName, true));
        return created;
    }

    public Document ValidateStatus(int documentId, string status, string notes)
    {
        var current = _documents.GetById(documentId);
        if (current == null)
        {
            throw new InvalidOperationException("Document was not found");
        }

        var normalized = status ?? string.Empty;
        if (normalized != "Pending" && normalized != "Accepted" && normalized != "Validated" && normalized != "Rejected" && normalized != "Missing")
        {
            throw new InvalidOperationException("Document status is not supported");
        }

        if ((normalized == "Rejected" || normalized == "Missing") && string.IsNullOrWhiteSpace(notes))
        {
            throw new InvalidOperationException("A note is required when a document is rejected");
        }

        var before = current.Status;
        current.Status = normalized;
        current.Notes = notes ?? string.Empty;
        var updated = _documents.Update(current);
        _logger.LogInformation("{Audit}", BuildAuditTrail("Document", "Validate", documentId, "admin", before, normalized, true));
        return updated;
    }

    private List<string> ValidateDocument(Document document)
    {
        var errors = CollectCommonFieldErrors(document?.FileName, document?.DocumentType, "files@example.test", "", 0, DateTime.UtcNow.Year, true, "Document");
        if (document == null)
        {
            return errors;
        }

        if (document.ApplicationId <= 0)
        {
            errors.Add("Application is required");
        }

        if (document.FileName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) || document.FileName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add("Executable metadata is not accepted");
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
    private int UnusedDocumentRank(string fileName, bool required, string status)
    {
        var unusedBase = 13;
        var unusedName = "legacy-document-rank";
        if (!required)
        {
            return unusedBase;
        }

        return fileName.Length + status.Length + unusedName.Length;
    }
}
