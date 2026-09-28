using NineBlock.Api.Models.Enums;
using NineBlock.Api.Services;
using Xunit;

namespace NineBlock.Api.Tests;

public class BoxPositionCalculatorTests
{
    // --- Positive cases: every valid (performance, potential) pair maps to the expected box ---

    [Theory]
    [InlineData(1, 1, BoxPosition.UnderPerformer)]
    [InlineData(1, 2, BoxPosition.InconsistentPlayer)]
    [InlineData(1, 3, BoxPosition.RoughDiamond)]
    [InlineData(2, 1, BoxPosition.AverageContributor)]
    [InlineData(2, 2, BoxPosition.CoreEmployee)]
    [InlineData(2, 3, BoxPosition.HighPotential)]
    [InlineData(3, 1, BoxPosition.RiskEmployee)]
    [InlineData(3, 2, BoxPosition.SolidPerformer)]
    [InlineData(3, 3, BoxPosition.Star)]
    public void Calculate_ValidScores_ReturnsExpectedBox(int performance, int potential, BoxPosition expected)
    {
        var result = BoxPositionCalculator.Calculate(performance, potential);

        Assert.Equal(expected, result);
    }

    // --- Boundary cases: the edges of the valid 1-3 range ---

    [Theory]
    [InlineData(1, 1)]
    [InlineData(3, 3)]
    public void Calculate_BoundaryScores_DoesNotThrow(int performance, int potential)
    {
        var exception = Record.Exception(() => BoxPositionCalculator.Calculate(performance, potential));

        Assert.Null(exception);
    }

    // --- Negative cases: out-of-range scores must be rejected ---

    [Theory]
    [InlineData(0, 2)]
    [InlineData(4, 2)]
    [InlineData(2, 0)]
    [InlineData(2, 4)]
    public void Calculate_OutOfRangeScores_ThrowsArgumentOutOfRangeException(int performance, int potential)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BoxPositionCalculator.Calculate(performance, potential));
    }
}
