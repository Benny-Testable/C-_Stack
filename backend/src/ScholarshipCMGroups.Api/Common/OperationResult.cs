namespace ScholarshipCMGroups.Api.Common;

/// <summary>
/// Outcome categories a service operation can report to a controller.
/// </summary>
public enum OperationStatus
{
    Success,
    NotFound,
    Conflict,
    ValidationFailed,
    Forbidden,
}

/// <summary>
/// Result of a service operation that yields a value.
/// </summary>
/// <remarks>
/// Services report expected failures (missing row, duplicate, illegal transition) through this
/// type instead of throwing. Exceptions are reserved for genuinely unexpected faults, which keeps
/// the per-method branch count low and makes every failure path directly unit-testable.
/// </remarks>
public readonly record struct OperationResult<T>
{
    internal OperationResult(OperationStatus status, T? value, string? error)
    {
        Status = status;
        Value = value;
        Error = error;
    }

    public OperationStatus Status { get; }

    public T? Value { get; }

    public string? Error { get; }

    public bool IsSuccess => Status == OperationStatus.Success;
}

/// <summary>
/// Factory methods for <see cref="OperationResult{T}"/>.
/// </summary>
/// <remarks>
/// Kept on a non-generic type so the factories are not static members of a generic type.
/// </remarks>
public static class OperationResult
{
    public static OperationResult<T> Success<T>(T value) => new(OperationStatus.Success, value, null);

    public static OperationResult<T> NotFound<T>(string error) => new(OperationStatus.NotFound, default, error);

    public static OperationResult<T> Conflict<T>(string error) => new(OperationStatus.Conflict, default, error);

    public static OperationResult<T> ValidationFailed<T>(string error) =>
        new(OperationStatus.ValidationFailed, default, error);

    public static OperationResult<T> Forbidden<T>(string error) => new(OperationStatus.Forbidden, default, error);
}
