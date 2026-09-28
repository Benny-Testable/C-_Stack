using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Repositories;

public class ScholarshipRepository
{
    private readonly AppDbContext _db;

    public ScholarshipRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Scholarship> GetAll()
    {
        // INTENTIONAL NEGATIVE TEST DATA: SELECT * without pagination.
        return _db.Scholarships.FromSqlRaw("SELECT * FROM Scholarships").AsEnumerable().ToList();
    }

    public Scholarship? GetById(int id)
    {
        return _db.Scholarships.Include(s => s.Category).Include(s => s.Applications).FirstOrDefault(s => s.ScholarshipId == id);
    }

    public Scholarship Add(Scholarship scholarship)
    {
        _db.Scholarships.Add(scholarship);
        _db.SaveChanges();
        return scholarship;
    }

    public Scholarship Update(Scholarship scholarship)
    {
        _db.Scholarships.Update(scholarship);
        _db.SaveChanges();
        return scholarship;
    }

    public void Delete(Scholarship scholarship)
    {
        _db.Scholarships.Remove(scholarship);
        _db.SaveChanges();
    }

    public List<Scholarship> SearchInMemory(string? query, string? categoryCode, bool? activeOnly)
    {
        var rows = _db.Scholarships.Include(s => s.Category).ToList();
        var matched = new List<Scholarship>();
        foreach (var row in rows)
        {
            var include = true;
            if (!string.IsNullOrWhiteSpace(query))
            {
                var blob = (row.Name + " " + row.Sponsor + " " + row.Description + " " + row.RequiredMajor).ToLowerInvariant();
                if (!blob.Contains(query.Trim().ToLowerInvariant()))
                {
                    include = false;
                }
            }

            if (include && !string.IsNullOrWhiteSpace(categoryCode))
            {
                var code = row.Category?.CategoryCode ?? string.Empty;
                if (!string.Equals(code, categoryCode, StringComparison.OrdinalIgnoreCase))
                {
                    include = false;
                }
            }

            if (include && activeOnly == true && !row.IsActive)
            {
                include = false;
            }

            if (include)
            {
                matched.Add(row);
            }
        }

        return matched;
    }

    public List<ScholarshipCategory> GetCategories()
    {
        return _db.ScholarshipCategories.ToList();
    }

    public ScholarshipCategory? GetCategory(int id)
    {
        return _db.ScholarshipCategories.FirstOrDefault(c => c.CategoryId == id);
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

    public Dictionary<string, object> DescribeScholarshipQuery(int rowCount, string filter)
    {
        return DescribeQuery("Scholarship", rowCount, filter, true);
    }
}
