namespace ScholarshipCMGroups.Models;

public class EligibilityResult
{
    public bool IsEligible { get; set; } = true;
    public int Score { get; set; }
    public string DecisionBand { get; set; } = "Review";
    public List<string> Reasons { get; set; } = new();
}

public class ProcessResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public Application? Application { get; set; }
    public EligibilityResult Eligibility { get; set; } = new();
    public List<string> Notes { get; set; } = new();
}

public class DashboardStats
{
    public int StudentCount { get; set; }
    public int ScholarshipCount { get; set; }
    public int ApplicationCount { get; set; }
    public int ApprovedCount { get; set; }
    public int RejectedCount { get; set; }
    public int PendingCount { get; set; }
    public int DocumentCount { get; set; }
    public decimal TotalAwarded { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}

public class ReportRow
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}
