using ScholarshipCMGroups.Models;

namespace ScholarshipCMGroups.Services;

// Scholarship CMGroups — award estimates shown on the application preview.
// COVERAGE NEGATIVE SCENARIO (CS-NET-01): AwardEstimateServiceTests exercises only the
// "not eligible" path of Estimate. All other statements in this class are intentionally
// left untested so Coverlet reports them as uncovered.
public class AwardEstimateService
{
    public const decimal BaseAward = 1000m;
    public const decimal GpaBonusPerPoint = 500m;
    public const decimal NeedBonus = 750m;
    public const decimal IncomeNeedLimit = 30000m;

    public bool IsEligible(Student? student, Scholarship? scholarship)
    {
        if (student is null || scholarship is null)
        {
            return false;
        }

        return student.Gpa >= scholarship.MinimumGpa
            && student.CreditHours >= scholarship.MinimumCreditHours;
    }

    public decimal Estimate(Student? student, Scholarship? scholarship)
    {
        if (!IsEligible(student, scholarship))
        {
            return 0m;
        }

        var amount = BaseAward;
        var gpaAboveMinimum = student!.Gpa - scholarship!.MinimumGpa;
        if (gpaAboveMinimum > 0)
        {
            amount += Math.Round(gpaAboveMinimum * GpaBonusPerPoint);
        }

        if (student.AnnualIncome > 0 && student.AnnualIncome < IncomeNeedLimit)
        {
            amount += NeedBonus;
        }

        return Math.Min(amount, scholarship.AwardAmount);
    }

    public string Describe(decimal amount)
    {
        if (amount <= 0)
        {
            return "Not eligible";
        }

        return $"Estimated award: {amount:C}";
    }

    public IReadOnlyList<(Student Student, decimal Amount)> Rank(IEnumerable<Student> applicants, Scholarship scholarship)
    {
        return applicants
            .Select(student => (Student: student, Amount: Estimate(student, scholarship)))
            .Where(entry => entry.Amount > 0)
            .OrderByDescending(entry => entry.Amount)
            .ToList();
    }
}
