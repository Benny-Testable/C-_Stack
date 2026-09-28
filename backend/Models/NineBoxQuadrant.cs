namespace NineBlock.Api.Models;

public class NineBoxQuadrant
{
    public int BlockNumber { get; set; } // 1 through 9
    public int CoordinateX { get; set; } // 1=Low, 2=Medium, 3=High (Performance)
    public int CoordinateY { get; set; } // 1=Low, 2=Medium, 3=High (Potential)
    public string QuadrantName { get; set; } = string.Empty; // e.g. "Star", "Enigma"
    public string PerformanceLevel { get; set; } = string.Empty;
    public string PotentialLevel { get; set; } = string.Empty;
    public string TalentTier { get; set; } = string.Empty; // High Value, Development, Specialist, Critical Risk
    public string ColorHex { get; set; } = "#FFFFFF";
    public string ActionPlan { get; set; } = string.Empty;
}
