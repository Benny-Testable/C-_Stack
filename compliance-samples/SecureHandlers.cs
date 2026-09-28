using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;

namespace NineBlock.Api.ComplianceSamples;

/// <summary>
/// Input is validated before use, no string-concatenated SQL (EF Core LINQ is
/// parameterized), no secrets or credentials in source, no eval/dynamic-code
/// execution, and no sensitive data is written to logs. Satisfies the Static
/// Vulnerabilities (SAST) metric group.
/// </summary>
public class SecureHandlers
{
    private readonly AppDbContext _db;

    public SecureHandlers(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> EmployeeExistsAsync(int employeeId)
    {
        if (employeeId <= 0)
            return false;

        return await _db.Employees.AnyAsync(e => e.Id == employeeId);
    }
}
