namespace NineBlock.Api.Models;

public class ReviewCycle
{
    public int Id { get; set; }
    public string CycleName { get; set; } = string.Empty; // e.g. "2026-Q1 Annual Review"
    public int Year { get; set; } = DateTime.UtcNow.Year;
    public string Quarter { get; set; } = "Q1";
    public bool IsActive { get; set; } = true;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}
