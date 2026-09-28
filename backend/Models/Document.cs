namespace ScholarshipCMGroups.Models;

public class Document
{
    public int DocumentId { get; set; }
    public int ApplicationId { get; set; }
    public Application? Application { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public bool IsRequired { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
