using NineBlock.Api.Models.Enums;

namespace NineBlock.Api.DTOs;

public record CreateAssessmentRequest(
    int EmployeeId,
    int ReviewCycleId,
    int PerformanceScore,
    int PotentialScore,
    int RatedById,
    string? Comments
);

public record UpdateAssessmentRequest(
    int PerformanceScore,
    int PotentialScore,
    string? Comments
);

public record AssessmentResponse(
    int Id,
    int EmployeeId,
    string EmployeeName,
    int ReviewCycleId,
    int PerformanceScore,
    int PotentialScore,
    BoxPosition BoxPosition,
    DateTime RatedDate,
    string? Comments
);
