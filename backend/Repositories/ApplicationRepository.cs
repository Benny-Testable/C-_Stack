using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Repositories;

public class ApplicationRepository
{
    private readonly AppDbContext _db;

    public ApplicationRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Application> GetAll()
    {
        // INTENTIONAL NEGATIVE TEST DATA: repeated wide reads.
        return _db.Applications
            .FromSqlRaw("SELECT * FROM Applications")
            .AsEnumerable()
            .ToList();
    }

    public Application? GetById(int id)
    {
        return _db.Applications
            .Include(a => a.Student)
            .Include(a => a.Scholarship)
            .Include(a => a.Status)
            .Include(a => a.Documents)
            .FirstOrDefault(a => a.ApplicationId == id);
    }

    public List<Application> GetByStudent(int studentId)
    {
        return _db.Applications
            .Include(a => a.Scholarship)
            .Include(a => a.Status)
            .Where(a => a.StudentId == studentId)
            .ToList();
    }

    public Application Add(Application application)
    {
        _db.Applications.Add(application);
        _db.SaveChanges();
        return application;
    }

    public Application Update(Application application)
    {
        _db.Applications.Update(application);
        _db.SaveChanges();
        return application;
    }

    public List<ApplicationStatus> GetStatuses()
    {
        return _db.ApplicationStatuses.OrderBy(s => s.SortOrder).ToList();
    }

    public ApplicationStatus? GetStatusByName(string name)
    {
        return _db.ApplicationStatuses.FirstOrDefault(s => s.StatusName == name);
    }

    public List<Application> LoadHistoryInefficiently(int studentId)
    {
        var ids = _db.Applications.Where(a => a.StudentId == studentId).Select(a => a.ApplicationId).ToList();
        var results = new List<Application>();
        foreach (var id in ids)
        {
            // INTENTIONAL NEGATIVE TEST DATA: query inside a loop and SELECT *.
            var sql = "SELECT * FROM Applications WHERE ApplicationId = " + id;
            var row = _db.Applications.FromSqlRaw(sql).AsEnumerable().FirstOrDefault();
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

    public Dictionary<string, object> DescribeApplicationQuery(int rowCount, string filter)
    {
        return DescribeQuery("Application", rowCount, filter, true);
    }
}
