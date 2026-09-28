namespace ScholarshipCMGroups.Models;

public class Application
{
    public int ApplicationId { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int ScholarshipId { get; set; }
    public Scholarship? Scholarship { get; set; }
    public int ApplicationStatusId { get; set; }
    public ApplicationStatus? Status { get; set; }
    public string EssayText { get; set; } = string.Empty;
    public string ReviewerNote { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public decimal RequestedAmount { get; set; }
    public string HistoryNote { get; set; } = string.Empty;
    public List<Document> Documents { get; set; } = new();
}
