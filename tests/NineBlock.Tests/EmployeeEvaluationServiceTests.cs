using NineBlock.Api.Services;
using Xunit;

namespace NineBlock.Tests;

public class EmployeeEvaluationServiceTests
{
    private readonly NineBoxMatrixService _matrixService = new();
    private readonly EmployeeEvaluationService _evaluationService;

    public EmployeeEvaluationServiceTests()
    {
        _evaluationService = new EmployeeEvaluationService(_matrixService);
    }

    [Theory]
    [InlineData(4.5, 4.5, 3, "Star")]
    [InlineData(1.5, 1.5, 7, "Risk")]
    [InlineData(3.5, 3.5, 5, "Core Player")]
    [InlineData(2.0, 4.2, 1, "Enigma")]
    [InlineData(4.8, 2.0, 9, "Solid Professional")]
    public void ResolveNineBoxQuadrant_ShouldDelegateToMatrixServiceConsistently(
        double perf, double pot, int expectedBlock, string expectedName)
    {
        var result = _evaluationService.ResolveNineBoxQuadrant((decimal)perf, (decimal)pot);

        Assert.Equal(expectedBlock, result.blockNumber);
        Assert.Equal(expectedName, result.quadrantName);
    }
}
