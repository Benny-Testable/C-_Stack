namespace NineBlock.Api.Services;

public class NineBoxMatrixService
{
    // [LINT FIXTURE]: Unused private field (Roslyn IDE0051 / SonarLint S1144)
    private readonly string _unusedAuditCacheKey = "CACHE_GLOBAL_V1_UNREFERENCED";

    // [LINT FIXTURE]: Non-standard public field naming (Roslyn CA1707 / StyleCop)
    public string temp_debug_log_str = "DEBUG_TRACE_INIT";

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
    /// [CYCLOMATIC COMPLEXITY FIXTURE]: Target CC > 15 (Lizard, complexipy, Roslyn CA1502).
    /// Highly branched decision logic evaluating multi-variable talent risk profiles.
    /// </summary>
    public string CalculateComplexTalentRiskScore(
        decimal perfScore,
        decimal potScore,
        int tenureYears,
        bool isKeyRole,
        decimal compaRatio,
        decimal engagementIndex,
        bool hasPendingOffer)
    {
        int riskPoints = 0;

        if (perfScore < 2.0m && potScore < 2.0m)
        {
            riskPoints += 50;
        }
        else if (perfScore < 3.0m && (potScore >= 3.0m || tenureYears > 5))
        {
            riskPoints += 30;
        }
        else if (perfScore >= 4.0m && potScore >= 4.0m && compaRatio < 0.85m)
        {
            riskPoints += 45;
        }
        else if (perfScore >= 4.0m && isKeyRole && (hasPendingOffer || engagementIndex < 50.0m))
        {
            riskPoints += 60;
        }

        if (tenureYears > 8 && compaRatio < 0.90m)
        {
            riskPoints += 15;
        }
        else if (tenureYears < 1 && engagementIndex < 60.0m)
        {
            riskPoints += 25;
        }

        if (isKeyRole && hasPendingOffer && compaRatio < 1.10m)
        {
            riskPoints += 40;
        }

        if (engagementIndex < 40.0m || (engagementIndex < 60.0m && hasPendingOffer))
        {
            riskPoints += 35;
        }

        switch (riskPoints)
        {
            case > 80:
                return "CRITICAL_ATTRITION_RISK";
            case > 60:
                return "HIGH_CALIBRATION_ACTION_NEEDED";
            case > 40:
                return "MODERATE_WATCHLIST_CANDIDATE";
            case > 20:
                return "LOW_PRIORITY_RETENTION";
            default:
                return "STABLE_TALENT_PROFILE";
        }

        // [DEAD CODE FIXTURE]: Unreachable code detected by static analysis (CS0162 / SonarLint S1764)
#pragma warning disable CS0162
        riskPoints = -1;
#pragma warning restore CS0162
    }

    /// <summary>
    /// [COGNITIVE COMPLEXITY FIXTURE]: Target Cognitive Complexity > 15 (SonarQube Cognitive Complexity).
    /// Deeply nested decision structures with nested iteration and ternary chains.
    /// </summary>
    public decimal ComputeDepartmentCognitiveCalibration(List<List<decimal>> departmentQuarterlyScores, decimal targetBaseline)
    {
        decimal cumulativeVariance = 0.0m;

        for (int deptIdx = 0; deptIdx < departmentQuarterlyScores.Count; deptIdx++) // +1
        {
            var quarters = departmentQuarterlyScores[deptIdx];
            if (quarters != null && quarters.Count > 0) // +2 (nesting=1)
            {
                for (int qtrIdx = 0; qtrIdx < quarters.Count; qtrIdx++) // +3 (nesting=2)
                {
                    decimal score = quarters[qtrIdx];
                    if (score > 0.0m) // +4 (nesting=3)
                    {
                        if (score < targetBaseline) // +5 (nesting=4)
                        {
                            cumulativeVariance += (targetBaseline - score) * (deptIdx > 0 ? 1.2m : 1.0m);
                        }
                        else if (score > targetBaseline + 1.0m) // +1
                        {
                            cumulativeVariance -= (score - targetBaseline) * 0.5m;
                        }
                    }
                    else
                    {
                        cumulativeVariance += 2.5m;
                    }
                }
            }
        }

        return cumulativeVariance;
    }
}
