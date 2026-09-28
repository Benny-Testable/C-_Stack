namespace NineBlock.Api.ComplianceSamples;

/// <summary>
/// Every method here is flat, single-purpose, and shallow (max nesting depth 1,
/// cyclomatic complexity <= 3) so it satisfies both the Cyclomatic Complexity and
/// Cognitive Complexity metric groups. Every declared variable is both defined and
/// used exactly once on a single path, satisfying All-Definition and All-Uses
/// coverage metrics.
/// </summary>
public static class CleanFunctions
{
    public static int Clamp(int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    public static decimal ApplyDiscount(decimal price, decimal discountPercent)
    {
        var discount = price * (discountPercent / 100m);
        return price - discount;
    }

    public static bool IsWithinRange(int value, int min, int max)
    {
        return value >= min && value <= max;
    }

    public static string FormatFullName(string firstName, string lastName)
    {
        return $"{firstName} {lastName}".Trim();
    }
}
