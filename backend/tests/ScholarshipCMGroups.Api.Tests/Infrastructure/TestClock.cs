using ScholarshipCMGroups.Api.Common;

namespace ScholarshipCMGroups.Api.Tests.Infrastructure;

/// <summary>
/// A clock frozen at a known instant.
/// </summary>
/// <remarks>
/// Every time-dependent rule in the domain (application windows, submission timestamps, token
/// expiry) reads <see cref="IClock"/>, so freezing it here makes those assertions deterministic
/// instead of dependent on the day the suite happens to run. The Excel "Flaky Test Rate" metric
/// expects 0%, and non-deterministic time is the most common cause of flakes.
/// </remarks>
internal sealed class TestClock : IClock
{
    public static readonly DateTimeOffset DefaultNow = new(2026, 6, 15, 12, 0, 0, TimeSpan.Zero);

    public TestClock()
        : this(DefaultNow)
    {
    }

    public TestClock(DateTimeOffset now) => UtcNow = now;

    public DateTimeOffset UtcNow { get; set; }
}
