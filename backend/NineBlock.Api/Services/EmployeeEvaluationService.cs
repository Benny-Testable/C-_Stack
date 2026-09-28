namespace NineBlock.Api.Services;

/// <summary>
/// Positive-case counterpart of the negative-cases branch's evaluation service:
/// delegates to <see cref="NineBoxMatrixService"/> instead of duplicating its
/// quadrant-resolution logic, so no clone exists for jscpd/SonarQube to detect.
/// </summary>
public class EmployeeEvaluationService
{
    private readonly NineBoxMatrixService _matrixService;

    public EmployeeEvaluationService(NineBoxMatrixService matrixService)
    {
        _matrixService = matrixService;
    }

    public (int blockNumber, string quadrantName, string colorHex) ResolveNineBoxQuadrant(decimal performanceScore, decimal potentialScore)
        => _matrixService.ResolveNineBoxQuadrant(performanceScore, potentialScore);
}
