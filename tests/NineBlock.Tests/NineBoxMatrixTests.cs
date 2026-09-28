using NineBlock.Api.Services;
using Xunit;

namespace NineBlock.Tests;

public class NineBoxMatrixTests
{
    /// <summary>
    /// Partial test suite deliberately covering only 1 happy path scenario.
    /// Leaves:
    /// - 8 out of 9 quadrants untested
    /// - CalculateComplexTalentRiskScore (CC=19) completely untested (0% branch coverage)
    /// - ComputeDepartmentCognitiveCalibration completely untested (0% path coverage)
    /// - EmployeeEvaluationService completely untested
    /// Resulting in intentional Statement Coverage < 30% and Branch Coverage < 25%.
    /// </summary>
    [Fact]
    public void ResolveNineBoxQuadrant_StarQuadrant_ReturnsBlockThree()
    {
        var service = new NineBoxMatrixService();

        var (block, name, color) = service.ResolveNineBoxQuadrant(4.5m, 4.5m);

        Assert.Equal(3, block);
        Assert.Equal("Star", name);
        Assert.Equal("#2ECC71", color);
    }
}
