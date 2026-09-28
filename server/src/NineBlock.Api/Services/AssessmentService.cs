using Microsoft.EntityFrameworkCore;
using NineBlock.Api.Data;
using NineBlock.Api.DTOs;
using NineBlock.Api.Models.Entities;

namespace NineBlock.Api.Services;

public class AssessmentService : IAssessmentService
{
    private readonly AppDbContext _db;

    public AssessmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AssessmentResponse> CreateAsync(CreateAssessmentRequest request)
    {
        var boxPosition = BoxPositionCalculator.Calculate(request.PerformanceScore, request.PotentialScore);

        var assessment = new Assessment
        {
            EmployeeId = request.EmployeeId,
            ReviewCycleId = request.ReviewCycleId,
            PerformanceScore = request.PerformanceScore,
            PotentialScore = request.PotentialScore,
            BoxPosition = boxPosition,
            RatedById = request.RatedById,
            RatedDate = DateTime.UtcNow,
            Comments = request.Comments
        };

        _db.Assessments.Add(assessment);
        await _db.SaveChangesAsync();

        var employee = await _db.Employees.FindAsync(request.EmployeeId);

        return new AssessmentResponse(
            assessment.Id,
            assessment.EmployeeId,
            employee?.Name ?? string.Empty,
            assessment.ReviewCycleId,
            assessment.PerformanceScore,
            assessment.PotentialScore,
            assessment.BoxPosition,
            assessment.RatedDate,
            assessment.Comments
        );
    }

    public async Task<AssessmentResponse?> UpdateAsync(int id, UpdateAssessmentRequest request)
    {
        var assessment = await _db.Assessments.Include(a => a.Employee).FirstOrDefaultAsync(a => a.Id == id);
        if (assessment is null) return null;

        assessment.PerformanceScore = request.PerformanceScore;
        assessment.PotentialScore = request.PotentialScore;
        assessment.BoxPosition = BoxPositionCalculator.Calculate(request.PerformanceScore, request.PotentialScore);
        assessment.Comments = request.Comments;
        assessment.RatedDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return new AssessmentResponse(
            assessment.Id,
            assessment.EmployeeId,
            assessment.Employee?.Name ?? string.Empty,
            assessment.ReviewCycleId,
            assessment.PerformanceScore,
            assessment.PotentialScore,
            assessment.BoxPosition,
            assessment.RatedDate,
            assessment.Comments
        );
    }

    public async Task<GridResponse?> GetGridAsync(int reviewCycleId, int? departmentId, int? managerId)
    {
        var cycle = await _db.ReviewCycles.FindAsync(reviewCycleId);
        if (cycle is null) return null;

        var query = _db.Assessments
            .Include(a => a.Employee)!.ThenInclude(e => e!.Department)
            .Where(a => a.ReviewCycleId == reviewCycleId);

        if (departmentId.HasValue)
            query = query.Where(a => a.Employee!.DepartmentId == departmentId.Value);

        if (managerId.HasValue)
            query = query.Where(a => a.Employee!.ManagerId == managerId.Value);

        var assessments = await query.ToListAsync();

        var boxes = new List<GridBox>();
        for (var perf = 1; perf <= 3; perf++)
        {
            for (var pot = 1; pot <= 3; pot++)
            {
                var position = BoxPositionCalculator.Calculate(perf, pot);
                var employees = assessments
                    .Where(a => a.PerformanceScore == perf && a.PotentialScore == pot)
                    .Select(a => new GridEmployeeCard(
                        a.EmployeeId,
                        a.Employee!.Name,
                        a.Employee.JobTitle,
                        a.Employee.Department?.Name ?? string.Empty,
                        a.Id))
                    .ToList();

                boxes.Add(new GridBox(position, perf, pot, employees));
            }
        }

        return new GridResponse(cycle.Id, cycle.Name, boxes);
    }
}
