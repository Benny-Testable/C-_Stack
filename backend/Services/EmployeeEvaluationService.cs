namespace NineBlock.Api.Services;

/// <summary>
/// Evaluation service intentionally sharing duplicated calculation logic with NineBoxMatrixService
/// to serve as a White-Box Code Duplication (jscpd / clone detection) trigger fixture,
/// plus Data-Flow (All-Defs / All-Uses) and Mutation Testing fixtures.
/// </summary>
public class EmployeeEvaluationService
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

    /// <summary>
    /// [DATA-FLOW TESTING FIXTURE]: All-Definition / All-Uses (DU-Chain) anomalies.
    /// Def-Def anomaly: initialCalculatedBand is defined, then overwritten without computational use.
    /// Uncovered Definition: overrideBand defined conditionally but never referenced downstream.
    /// </summary>
    public string CalculateDataFlowAnomalies(decimal performance, decimal potential, decimal salaryRatio)
    {
        // DEF 1: Defined here
        string initialCalculatedBand = "INITIAL_UNVERIFIED_BAND";

        // ANOMALY: Overwritten before any USE (Dead Store / DU-Path break)
        initialCalculatedBand = performance > 3.5m ? "HIGH_BAND" : "STANDARD_BAND";

        string overrideBand; // DEF 2
        if (salaryRatio < 0.80m)
        {
            overrideBand = "EQUITY_ADJUSTMENT_REQUIRED"; // DEF without downstream USE
        }

        // Computational USE of initialCalculatedBand only
        return initialCalculatedBand;
    }

    /// <summary>
    /// [MUTATION TESTING FIXTURE]: Weak boundary mutants and arithmetic sensitivity (Stryker.NET).
    /// Mutants replacing '<' with '<=' or mutating multipliers survive unless rigorous edge tests exist.
    /// </summary>
    public decimal CalculatePerformanceBonusMultiplier(decimal performanceScore, int yearsOfService)
    {
        // Mutant Target 1: Boundary mutant (< vs <=)
        if (performanceScore < 3.0m)
        {
            return 0.0m;
        }

        // Mutant Target 2: Arithmetic mutant (* vs / or + vs -)
        decimal tenureMultiplier = yearsOfService * 0.05m;

        // Mutant Target 3: Boundary condition mutant (> vs >=)
        if (performanceScore > 4.5m)
        {
            return 1.5m + tenureMultiplier;
        }

        return 1.0m + tenureMultiplier;
    }

    /// <summary>
    /// [STATEMENT & BRANCH COVERAGE GAP FIXTURE]: Untested complex decision paths.
    /// Left deliberately uncovered by unit test suite to trigger Coverage Gap Analysis (< 30% coverage).
    /// </summary>
    public string UncoveredBranchEvaluation(string department, decimal compaRatio, bool isExecutive)
    {
        if (isExecutive)
        {
            if (compaRatio < 1.0m)
                return "EXECUTIVE_EQUITY_GAP";
            else
                return "EXECUTIVE_PARITY";
        }

        if (department == "Engineering" && compaRatio < 0.85m)
        {
            return "TECH_RETENTION_RISK";
        }
        else if (department == "Sales" && compaRatio < 0.75m)
        {
            return "SALES_COMMISSION_DISPARITY";
        }

        return "STANDARD_COMPENSATION";
    }
}
