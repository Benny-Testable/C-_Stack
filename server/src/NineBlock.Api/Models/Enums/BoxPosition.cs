namespace NineBlock.Api.Models.Enums;

/// <summary>
/// 3x3 grid position: Performance (Low/Medium/High) x Potential (Low/Medium/High).
/// </summary>
public enum BoxPosition
{
    UnderPerformer,       // Low Performance / Low Potential
    InconsistentPlayer,   // Low Performance / Medium Potential
    RoughDiamond,         // Low Performance / High Potential
    AverageContributor,   // Medium Performance / Low Potential
    CoreEmployee,         // Medium Performance / Medium Potential
    HighPotential,        // Medium Performance / High Potential
    RiskEmployee,         // High Performance / Low Potential
    SolidPerformer,       // High Performance / Medium Potential
    Star                  // High Performance / High Potential
}
