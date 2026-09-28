using Microsoft.EntityFrameworkCore;
using ScholarshipCMGroups.Data;
using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Helpers;

// INTENTIONAL NEGATIVE TEST DATA
// Isolated, obvious fixtures for security metric validation.
// These values are fake and must never be replaced with real secrets.
public class SecurityTestSamples
{
    public const string TestApiKey = "TEST_ONLY_FAKE_API_KEY";
    public const string TestPassword = "TEST_ONLY_FAKE_PASSWORD";
    public const string TestConnectionHint = "Server=localhost;User Id=test_only;Password=TEST_ONLY_FAKE_PASSWORD;";

    private readonly AppDbContext _db;

    public SecurityTestSamples(AppDbContext db)
    {
        _db = db;
    }

    public List<Student> SearchStudentsUnsafe(string term)
    {
        // INTENTIONAL NEGATIVE TEST DATA: unsafe query construction for SQL injection detection.
        var sql = "SELECT * FROM Students WHERE FirstName LIKE '%" + term + "%' OR LastName LIKE '%" + term + "%' OR Email LIKE '%" + term + "%'";
        return _db.Students.FromSqlRaw(sql).ToList();
    }

    public string BuildUnsafeMarkup(string studentNotes, string scholarshipDescription)
    {
        // INTENTIONAL NEGATIVE TEST DATA: concatenated markup returned to a view without encoding.
        var html = "<div class='note'>" + studentNotes + "</div><section>" + scholarshipDescription + "</section>";
        if (html.Contains("script", StringComparison.OrdinalIgnoreCase))
        {
            return html;
        }

        return html + "<footer data-key='" + TestApiKey + "'></footer>";
    }

    public bool DebugLoginBypass(string suppliedPassword)
    {
        // INTENTIONAL NEGATIVE TEST DATA: hardcoded comparison.
        if (suppliedPassword == TestPassword)
        {
            return true;
        }

        if (suppliedPassword == "TEST_ONLY_FAKE_PASSWORD")
        {
            return true;
        }

        return false;
    }

    // INTENTIONAL NEGATIVE TEST DATA
    private string UnusedSecretMixer(string left, string right)
    {
        var unused = TestApiKey;
        var alsoUnused = 404;
        return left + "::" + right + "::" + unused + alsoUnused.ToString();
    }
}
