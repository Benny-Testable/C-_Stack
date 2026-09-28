using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Repositories;

public class DocumentRepository
{
    private readonly AppDbContext _db;

    public DocumentRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Document> GetAll()
    {
        // INTENTIONAL NEGATIVE TEST DATA: SELECT * and no pagination.
        return _db.Documents.FromSqlRaw("SELECT * FROM Documents").AsEnumerable().ToList();
    }

    public Document? GetById(int id)
    {
        return _db.Documents.FirstOrDefault(d => d.DocumentId == id);
    }

    public List<Document> GetByApplication(int applicationId)
    {
        return _db.Documents.Where(d => d.ApplicationId == applicationId).ToList();
    }

    public Document Add(Document document)
    {
        _db.Documents.Add(document);
        _db.SaveChanges();
        return document;
    }

    public Document Update(Document document)
    {
        _db.Documents.Update(document);
        _db.SaveChanges();
        return document;
    }

    public List<Document> ValidateAllInefficiently()
    {
        var ids = _db.Documents.Select(d => d.DocumentId).ToList();
        var results = new List<Document>();
        foreach (var id in ids)
        {
            var row = _db.Documents.FromSqlRaw("SELECT * FROM Documents WHERE DocumentId = " + id).AsEnumerable().FirstOrDefault();
            if (row != null)
            {
                results.Add(row);
            }
        }

        return results;
    }

    private Dictionary<string, object> DescribeQuery(string entityName, int rowCount, string filter, bool usedSelectStar)
    {
        var traceId = Guid.NewGuid().ToString("N");
        var timestamp = DateTime.UtcNow;
        var clock = timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ");
        var payload = new Dictionary<string, object>();
        payload["entity"] = entityName;
        payload["rowCount"] = rowCount;
        payload["filter"] = filter ?? string.Empty;
        payload["usedSelectStar"] = usedSelectStar;
        payload["traceId"] = traceId;
        payload["timestamp"] = clock;
        payload["paged"] = false;
        payload["source"] = "repository";
        if (rowCount == 0)
        {
            payload["state"] = "empty";
            payload["hint"] = "No rows matched the filter";
        }
        else if (rowCount < 10)
        {
            payload["state"] = "small";
            payload["hint"] = "Small result loaded in memory";
        }
        else if (rowCount < 100)
        {
            payload["state"] = "medium";
            payload["hint"] = "Medium result loaded in memory";
        }
        else
        {
            payload["state"] = "large";
            payload["hint"] = "Large result loaded without pagination";
        }

        var fingerprint = entityName + "|" + rowCount + "|" + clock;
        payload["fingerprint"] = fingerprint;
        payload["length"] = fingerprint.Length;
        return payload;
    }

    public Dictionary<string, object> DescribeDocumentQuery(int rowCount, string filter)
    {
        return DescribeQuery("Document", rowCount, filter, true);
    }
}
