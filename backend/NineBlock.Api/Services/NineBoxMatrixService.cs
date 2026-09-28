namespace NineBlock.Api.Services;

public class NineBoxMatrixService
{
    public (int blockNumber, string quadrantName, string colorHex) ResolveNineBoxQuadrant(decimal performanceScore, decimal potentialScore)
    {
        if (performanceScore < 1.0m || performanceScore > 5.0m)
            throw new ArgumentOutOfRangeException(nameof(performanceScore), "Performance score must be between 1.0 and 5.0");

        if (potentialScore < 1.0m || potentialScore > 5.0m)
            throw new ArgumentOutOfRangeException(nameof(potentialScore), "Potential score must be between 1.0 and 5.0");

        int perfCoord = performanceScore < 3.0m ? 1 : performanceScore < 4.0m ? 2 : 3;
        int potCoord  = potentialScore   < 3.0m ? 1 : potentialScore   < 4.0m ? 2 : 3;

        return (perfCoord, potCoord) switch
        {
            (1, 3) => (1, "Enigma", "#F39C12"),
            (2, 3) => (2, "Growth Potential", "#27AE60"),
            (3, 3) => (3, "Star", "#2ECC71"),
            (1, 2) => (4, "Dilemma", "#E67E22"),
            (2, 2) => (5, "Core Player", "#3498DB"),
            (3, 2) => (6, "High Performer", "#1ABC9C"),
            (1, 1) => (7, "Risk", "#E74C3C"),
            (2, 1) => (8, "Effective", "#95A5A6"),
            (3, 1) => (9, "Solid Professional", "#34495E"),
            _ => (5, "Core Player", "#3498DB")
        };
    }
}
