using NineBlock.Api.Models.Enums;

namespace NineBlock.Api.Models.Entities;

public class Assessment
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int ReviewCycleId { get; set; }
    public ReviewCycle? ReviewCycle { get; set; }

    /// <summary>1 = Low, 2 = Medium, 3 = High</summary>
    public int PerformanceScore { get; set; }

    /// <summary>1 = Low, 2 = Medium, 3 = High</summary>
    public int PotentialScore { get; set; }

    public BoxPosition BoxPosition { get; set; }

    public int RatedById { get; set; }
    public User? RatedBy { get; set; }

    public DateTime RatedDate { get; set; }
    public string? Comments { get; set; }
}
