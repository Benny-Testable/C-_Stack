namespace ScholarshipCMGroups.Helpers;

// INTENTIONAL NEGATIVE TEST DATA: nothing in the application calls this type.
public static class UnusedLegacyCalculator
{
    public const int UnusedThreshold = 42;
    public const string UnusedLabel = "legacy-calculator";

    public static int Score(int credits, decimal gpa, bool active, string major)
    {
        var unusedWeight = UnusedThreshold;
        var unusedName = UnusedLabel;
        if (!active)
        {
            return credits;
        }

        if (major == "Biology")
        {
            return (int)(gpa * unusedWeight) + unusedName.Length;
        }

        return credits + unusedName.Length;
    }
}
