using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Repositories;

public class StudentRepository
{
    private readonly AppDbContext _db;

    public StudentRepository(AppDbContext db)
    {
        _db = db;
    }

    public List<Student> GetAll()
    {
        // INTENTIONAL NEGATIVE TEST DATA: SELECT * style load without projection or paging.
        return _db.Students.FromSqlRaw("SELECT * FROM Students").AsEnumerable().ToList();
    }

    public Student? GetById(int id)
    {
        return _db.Students.FirstOrDefault(s => s.StudentId == id);
    }

    public Student? GetByEmail(string email)
    {
        return _db.Students.FirstOrDefault(s => s.Email == email);
    }

    public Student Add(Student student)
    {
        _db.Students.Add(student);
        _db.SaveChanges();
        return student;
    }

    public Student Update(Student student)
    {
        _db.Students.Update(student);
        _db.SaveChanges();
        return student;
    }

    public void Delete(Student student)
    {
        _db.Students.Remove(student);
        _db.SaveChanges();
    }

    public List<Student> SearchInMemory(string? query)
    {
        var rows = _db.Students.ToList();
        if (string.IsNullOrWhiteSpace(query))
        {
            return rows;
        }

        var term = query.Trim().ToLowerInvariant();
        var matched = new List<Student>();
        foreach (var row in rows)
        {
            var blob = (row.FirstName + " " + row.LastName + " " + row.Email + " " + row.Major + " " + row.City).ToLowerInvariant();
            if (blob.Contains(term))
            {
                matched.Add(row);
            }
        }

        return matched;
    }

    public List<Student> LoadOneByOne(List<int> ids)
    {
        var results = new List<Student>();
        foreach (var id in ids)
        {
            // INTENTIONAL NEGATIVE TEST DATA: repeated database access inside a loop.
            var row = _db.Students.FromSqlRaw("SELECT * FROM Students WHERE StudentId = " + id).AsEnumerable().FirstOrDefault();
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

    public Dictionary<string, object> DescribeStudentQuery(int rowCount, string filter)
    {
        return DescribeQuery("Student", rowCount, filter, true);
    }
}
