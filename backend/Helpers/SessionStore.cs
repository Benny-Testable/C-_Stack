namespace ScholarshipCMGroups.Helpers;

public class SessionStore
{
    private readonly Dictionary<string, SessionRecord> _tokens = new();

    public string Issue(int userId, string role, string displayName)
    {
        var token = Guid.NewGuid().ToString("N");
        _tokens[token] = new SessionRecord
        {
            UserId = userId,
            Role = role,
            DisplayName = displayName,
            IssuedAt = DateTime.UtcNow
        };
        return token;
    }

    public bool IsValid(string? token, string? requiredRole)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        if (!_tokens.TryGetValue(token, out var record))
        {
            return false;
        }

        if (record.IssuedAt < DateTime.UtcNow.AddHours(-8))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(requiredRole) && !string.Equals(record.Role, requiredRole, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    public SessionRecord? Find(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return _tokens.TryGetValue(token, out var record) ? record : null;
    }

    private sealed class UnusedSessionCleanup
    {
        // INTENTIONAL NEGATIVE TEST DATA
        public int UnusedTimeoutMinutes { get; set; } = 42;
    }
}

public class SessionRecord
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
}
