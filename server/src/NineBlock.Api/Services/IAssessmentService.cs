using NineBlock.Api.DTOs;

namespace NineBlock.Api.Services;

public interface IAssessmentService
{
    Task<AssessmentResponse> CreateAsync(CreateAssessmentRequest request);
    Task<AssessmentResponse?> UpdateAsync(int id, UpdateAssessmentRequest request);
    Task<GridResponse?> GetGridAsync(int reviewCycleId, int? departmentId, int? managerId);
}
