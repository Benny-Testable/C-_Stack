namespace WestCoastFitness.Domain;

public sealed class ClubRuleException : Exception
{
    public ClubRuleException(string message)
        : base(message)
    {
    }
}
