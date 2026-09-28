namespace NineBlock.Api.Models;

public class Assessment
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int ReviewCycleId { get; set; }
    public ReviewCycle? ReviewCycle { get; set; }

    public decimal PerformanceScore { get; set; } // 1.0 to 5.0
    public decimal PotentialScore { get; set; }   // 1.0 to 5.0

    public int CoordinateX { get; set; } // 1, 2, or 3
    public int CoordinateY { get; set; } // 1, 2, or 3
    public int AssignedBlockNumber { get; set; } // 1 to 9
    public string AssignedQuadrantName { get; set; } = string.Empty;

    public string EvaluatorNotes { get; set; } = string.Empty;
    public string CalibrationNotes { get; set; } = string.Empty;
    public bool IsCalibrated { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CalibratedAt { get; set; }
}
