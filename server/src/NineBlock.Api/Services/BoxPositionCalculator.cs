using NineBlock.Api.Models.Enums;

namespace NineBlock.Api.Services;

/// <summary>
/// Maps a (performance, potential) pair — each 1=Low, 2=Medium, 3=High — to one of the 9 grid boxes.
/// </summary>
public static class BoxPositionCalculator
{
    private static readonly BoxPosition[,] Grid =
    {
        // potential:      Low                          Medium                      High
        /* perf Low  */ { BoxPosition.UnderPerformer,   BoxPosition.InconsistentPlayer, BoxPosition.RoughDiamond },
        /* perf Med  */ { BoxPosition.AverageContributor, BoxPosition.CoreEmployee,     BoxPosition.HighPotential },
        /* perf High */ { BoxPosition.RiskEmployee,      BoxPosition.SolidPerformer,     BoxPosition.Star }
    };

    public static BoxPosition Calculate(int performanceScore, int potentialScore)
    {
        if (performanceScore is < 1 or > 3)
            throw new ArgumentOutOfRangeException(nameof(performanceScore), "Performance score must be between 1 and 3.");
        if (potentialScore is < 1 or > 3)
            throw new ArgumentOutOfRangeException(nameof(potentialScore), "Potential score must be between 1 and 3.");

        return Grid[performanceScore - 1, potentialScore - 1];
    }
}
