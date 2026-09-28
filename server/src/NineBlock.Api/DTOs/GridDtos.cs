using NineBlock.Api.Models.Enums;

namespace NineBlock.Api.DTOs;

public record GridEmployeeCard(int EmployeeId, string Name, string JobTitle, string Department, int AssessmentId);

public record GridBox(BoxPosition Position, int PerformanceLevel, int PotentialLevel, List<GridEmployeeCard> Employees);

public record GridResponse(int ReviewCycleId, string ReviewCycleName, List<GridBox> Boxes);
