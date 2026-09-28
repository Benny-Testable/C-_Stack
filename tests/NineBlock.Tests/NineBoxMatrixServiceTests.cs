using NineBlock.Api.Services;
using Xunit;

namespace NineBlock.Tests;

public class NineBoxMatrixServiceTests
{
    private readonly NineBoxMatrixService _service = new();

    [Theory]
    // Block 1: Enigma (Perf: Low, Pot: High)
    [InlineData(1.5, 4.5, 1, "Enigma", "#F39C12")]
    [InlineData(2.99, 4.0, 1, "Enigma", "#F39C12")]
    [InlineData(1.0, 5.0, 1, "Enigma", "#F39C12")]
    // Block 2: Growth Potential (Perf: Medium, Pot: High)
    [InlineData(3.0, 4.5, 2, "Growth Potential", "#27AE60")]
    [InlineData(3.5, 4.0, 2, "Growth Potential", "#27AE60")]
    [InlineData(3.99, 5.0, 2, "Growth Potential", "#27AE60")]
    // Block 3: Star (Perf: High, Pot: High)
    [InlineData(4.0, 4.0, 3, "Star", "#2ECC71")]
    [InlineData(4.5, 4.8, 3, "Star", "#2ECC71")]
    [InlineData(5.0, 5.0, 3, "Star", "#2ECC71")]
    // Block 4: Dilemma (Perf: Low, Pot: Medium)
    [InlineData(1.0, 3.0, 4, "Dilemma", "#E67E22")]
    [InlineData(2.5, 3.5, 4, "Dilemma", "#E67E22")]
    [InlineData(2.99, 3.99, 4, "Dilemma", "#E67E22")]
    // Block 5: Core Player (Perf: Medium, Pot: Medium)
    [InlineData(3.0, 3.0, 5, "Core Player", "#3498DB")]
    [InlineData(3.5, 3.5, 5, "Core Player", "#3498DB")]
    [InlineData(3.99, 3.99, 5, "Core Player", "#3498DB")]
    // Block 6: High Performer (Perf: High, Pot: Medium)
    [InlineData(4.0, 3.0, 6, "High Performer", "#1ABC9C")]
    [InlineData(4.5, 3.5, 6, "High Performer", "#1ABC9C")]
    [InlineData(5.0, 3.99, 6, "High Performer", "#1ABC9C")]
    // Block 7: Risk (Perf: Low, Pot: Low)
    [InlineData(1.0, 1.0, 7, "Risk", "#E74C3C")]
    [InlineData(2.0, 2.5, 7, "Risk", "#E74C3C")]
    [InlineData(2.99, 2.99, 7, "Risk", "#E74C3C")]
    // Block 8: Effective (Perf: Medium, Pot: Low)
    [InlineData(3.0, 1.0, 8, "Effective", "#95A5A6")]
    [InlineData(3.5, 2.5, 8, "Effective", "#95A5A6")]
    [InlineData(3.99, 2.99, 8, "Effective", "#95A5A6")]
    // Block 9: Solid Professional (Perf: High, Pot: Low)
    [InlineData(4.0, 1.0, 9, "Solid Professional", "#34495E")]
    [InlineData(4.5, 2.0, 9, "Solid Professional", "#34495E")]
    [InlineData(5.0, 2.99, 9, "Solid Professional", "#34495E")]
    public void ResolveNineBoxQuadrant_ShouldMapAllNineQuadrantsCorrectly(
        double perf, double pot, int expectedBlock, string expectedName, string expectedColor)
    {
        // Act
        var (block, name, color) = _service.ResolveNineBoxQuadrant((decimal)perf, (decimal)pot);

        // Assert
        Assert.Equal(expectedBlock, block);
        Assert.Equal(expectedName, name);
        Assert.Equal(expectedColor, color);
    }

    [Theory]
    [InlineData(0.9, 3.0)]
    [InlineData(5.1, 3.0)]
    [InlineData(0.0, 4.0)]
    [InlineData(-1.0, 2.0)]
    public void ResolveNineBoxQuadrant_ShouldThrowWhenPerformanceScoreIsOutOfRange(double perf, double pot)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.ResolveNineBoxQuadrant((decimal)perf, (decimal)pot));

        Assert.Equal("performanceScore", ex.ParamName);
    }

    [Theory]
    [InlineData(3.0, 0.9)]
    [InlineData(3.0, 5.1)]
    [InlineData(4.0, 0.0)]
    [InlineData(2.0, -2.5)]
    public void ResolveNineBoxQuadrant_ShouldThrowWhenPotentialScoreIsOutOfRange(double perf, double pot)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.ResolveNineBoxQuadrant((decimal)perf, (decimal)pot));

        Assert.Equal("potentialScore", ex.ParamName);
    }
}
