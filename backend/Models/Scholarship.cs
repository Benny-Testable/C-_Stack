namespace ScholarshipCMGroups.Models;

public class Scholarship
{
    public int ScholarshipId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Sponsor { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public ScholarshipCategory? Category { get; set; }
    public decimal AwardAmount { get; set; }
    public decimal MinimumGpa { get; set; }
    public int MinimumCreditHours { get; set; }
    public decimal MaximumIncome { get; set; }
    public string RequiredMajor { get; set; } = string.Empty;
    public string RequiredResidency { get; set; } = string.Empty;
    public bool RequiresEssay { get; set; }
    public bool RequiresTranscript { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime Deadline { get; set; }
    public int Seats { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<Application> Applications { get; set; } = new();
}
