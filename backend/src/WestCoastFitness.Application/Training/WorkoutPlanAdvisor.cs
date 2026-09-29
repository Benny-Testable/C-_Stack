using WestCoastFitness.Domain;

namespace WestCoastFitness.Application;

public static class WorkoutPlanAdvisor
{
    public static WorkoutPlan Recommend(Guid memberId, IReadOnlyList<FitnessAssessment> history, DateOnly today)
    {
        ArgumentNullException.ThrowIfNull(history);

        FitnessAssessment? latest = null;
        foreach (var row in history)
        {
            if (latest is null || row.AssessedOn > latest.AssessedOn)
            {
                latest = row;
            }
        }

        var sessions = 3;
        var title = "Desert base conditioning";
        if (latest is not null)
        {
            if (latest.BodyMassIndex >= 30m && latest.RestingHeartRate >= 80)
            {
                sessions = 2;
                title = "Recovery and walking intervals";
            }
            else if (latest.RestingHeartRate < 60 && latest.BodyMassIndex < 25m)
            {
                sessions = 5;
                title = "Canyon performance block";
            }
            else if (latest.RestingHeartRate >= 90)
            {
                sessions = 2;
                title = "Heart-rate limited circuit";
            }
        }

        return new WorkoutPlan
        {
            Id = Guid.NewGuid(),
            MemberId = memberId,
            Title = title,
            SessionsPerWeek = sessions,
            CreatedOn = today,
        };
    }
}
