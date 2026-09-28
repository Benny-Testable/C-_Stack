namespace ScholarshipCMGroups.Models;

public class ScholarshipCategory
{
    public int CategoryId { get; set; }
    public string CategoryCode { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public List<Scholarship> Scholarships { get; set; } = new();
}
