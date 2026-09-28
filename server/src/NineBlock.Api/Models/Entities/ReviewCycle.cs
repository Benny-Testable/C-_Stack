using NineBlock.Api.Models.Enums;

namespace NineBlock.Api.Models.Entities;

public class ReviewCycle
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CycleStatus Status { get; set; } = CycleStatus.Draft;

    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}
