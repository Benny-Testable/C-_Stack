using ScholarshipCMGroups.Models;
using ScholarshipCMGroups.Services;
using Xunit;

namespace ScholarshipCMGroups.Tests;

// COVERAGE NEGATIVE SCENARIO (CS-NET-01): only the "no student" path is exercised so the
// Coverlet statement (line) threshold of 80% is intentionally not met.
public class AwardEstimateServiceTests
{
    [Fact]
    public void Estimate_WithoutStudent_ReturnsZero()
    {
        var service = new AwardEstimateService();
        var scholarship = new Scholarship { MinimumGpa = 3.0m, MinimumCreditHours = 12, AwardAmount = 5000m };

        Assert.Equal(0m, service.Estimate(null, scholarship));
    }
}
