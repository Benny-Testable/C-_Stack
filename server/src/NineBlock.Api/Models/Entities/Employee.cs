namespace NineBlock.Api.Models.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int? ManagerId { get; set; }
    public Employee? Manager { get; set; }

    public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
}
