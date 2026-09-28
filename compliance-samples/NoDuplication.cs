namespace NineBlock.Api.ComplianceSamples;

/// <summary>
/// Common logic is abstracted into a single shared method instead of being
/// copy-pasted, so there are zero cloned blocks/tokens in this file — satisfies
/// every Code Duplication metric (duplication %, clone clusters, abstraction
/// potential, etc.).
/// </summary>
public static class NoDuplication
{
    public static bool IsValidPerformanceScore(int score) => IsInBand(score);

    public static bool IsValidPotentialScore(int score) => IsInBand(score);

    private static bool IsInBand(int score) => score is >= 1 and <= 3;
}
