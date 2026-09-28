namespace ScholarshipCMGroups.Api.Common;

/// <summary>
/// Supplies the current time. Injected so that time-dependent behaviour is deterministic in tests.
/// </summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }

    DateOnly Today => DateOnly.FromDateTime(UtcNow.UtcDateTime);
}

/// <summary>Reads the machine clock.</summary>
public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
