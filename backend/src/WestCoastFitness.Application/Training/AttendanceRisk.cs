using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public static class AttendanceRisk
{
    public static int Score(IReadOnlyList<BookingStatus> recentOutcomes)
    {
        ArgumentNullException.ThrowIfNull(recentOutcomes);
        var score = 0;
        foreach (var outcome in recentOutcomes)
        {
            if (outcome == BookingStatus.NoShow)
            {
                score += 3;
            }
            else if (outcome == BookingStatus.Cancelled)
            {
                score += 1;
            }
            else if (outcome == BookingStatus.Attended)
            {
                score = Math.Max(0, score - 1);
            }
        }

        return score;
    }
}
