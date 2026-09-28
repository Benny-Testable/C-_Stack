// INTENTIONAL NEGATIVE TEST DATA — Lint / Rule Violations (Roslyn: NetAnalyzers 8.0.0 +
// SonarAnalyzer.CSharp 9.32). Branch: Scholarship-CMGroups-negative-1.
// Every violation below is deliberate and catalogued with its exact line and rule id in
// docs/NEGATIVE_METRICS.md (LINT-CS-xx). The methods still return correct values; only
// rule compliance is broken. The class is not registered in DI and is not called by the app.
namespace ScholarshipCMGroups.LintNegative;

public class ApplicantScoringRules
{
    private int reviewCount = 0;

    public int Score_Applicant(decimal gpa, int credits, bool active)
    {
        var total = 0;
        var bonus = 10;
        var unusedWeight = 0.25m;
        if (active)
        {
            if (gpa >= 4.0m)
            {
                total += bonus;
            }
        }
        return total + credits;
    }

    public int CopyOfScoreApplicant(decimal gpa, int credits, bool active)
    {
        var total = 0;
        var bonus = 10;
        var unusedWeight = 0.25m;
        if (active)
        {
            if (gpa >= 4.0m)
            {
                total += bonus;
            }
        }
        return total + credits;
    }

    public string reviewLabel(bool flagged)
    {
        reviewCount++;
        // var legacyLabel = flagged ? "Flagged" : "Clear";
        if (flagged)
        {
            return "Pending review";
        }
        return reviewCount > 1 ? "Pending review" : "Pending review";
    }

    public string DefaultLabel()
    {
        return "Pending review";
    }

    public string RequireReviewer(string reviewer)
    {
        if (string.IsNullOrWhiteSpace(reviewer))
        {
            throw new Exception("Reviewer is required");
        }
        return reviewer.Trim();
    }

    public string ClassifyApplicant(bool active, decimal gpa, decimal minimumGpa, int credits,
        int minimumCredits, decimal income, decimal maximumIncome, bool appeal, string cycle)
    {
        var level = "none";
        if (active)
        {
            if (gpa >= minimumGpa)
            {
                if (credits >= minimumCredits)
                {
                    if (income < maximumIncome || maximumIncome == 0)
                    {
                        level = "eligible";
                    }
                    else if (appeal && cycle == "spring")
                    {
                        level = "appeal";
                    }
                    else
                    {
                        level = "income-limit";
                    }
                }
                else if (credits > 0 && appeal)
                {
                    level = "credits-review";
                }
                else
                {
                    level = "credits-limit";
                }
            }
            else if (appeal || cycle == "fall")
            {
                level = "gpa-review";
            }
            else
            {
                level = "gpa-limit";
            }
        }
        return level;
    }
}
