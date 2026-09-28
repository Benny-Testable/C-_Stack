namespace ScholarshipCMGroups.Models;

public class ApplicationStatus
{
    public int ApplicationStatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<Application> Applications { get; set; } = new();
}
